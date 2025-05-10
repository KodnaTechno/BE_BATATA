using AppWorkflow.Domain.Schema;
using AppWorkflow.Engine;
using AppWorkflow.Infrastructure.Data.Context;
using AppWorkflow.Infrastructure.Repositories.IRepository;
using AppWorkflow.Infrastructure.Services.Actions;
using AppWorkflow.Services.Interfaces;
using AppWorkflow.Core.Models;
using AppWorkflow.Infrastructure.Services;
using AppWorkflow.Core.Interfaces.Services;

namespace AppWorkflow.Engine;

public class WorkflowEngine: IWorkflowEngine
{
    private readonly IActionResolver _actionResolver;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IWorkflowDataRepository _instanceRepository;
    private readonly IExpressionEvaluator _expressionEvaluator;
    //private readonly IStepExecutorFactory _stepExecutorFactory;
    //private readonly IWorkflowStateManager _stateManager;
    private readonly IWorkflowEventHandler _eventHandler;
    private readonly IDistributedLockManager _lockManager;
    private readonly ILogger<WorkflowEngine> _logger;
    private readonly ITelemetryTracker _telemetry;
    private readonly IServiceProvider _serviceProvider;

    public WorkflowEngine(
        IActionResolver actionResolver,
        IWorkflowRepository workflowRepository,
        IWorkflowDataRepository instanceRepository,
        IExpressionEvaluator expressionEvaluator,
        IStepExecutorFactory stepExecutorFactory,
        IWorkflowStateManager stateManager,
        IWorkflowEventHandler eventHandler,
        IDistributedLockManager lockManager,
        ITelemetryTracker telemetry,
        ILogger<WorkflowEngine> logger,
        IServiceProvider serviceProvider)
    {
        _actionResolver = actionResolver;
        _workflowRepository = workflowRepository;
        _instanceRepository = instanceRepository;
        _expressionEvaluator = expressionEvaluator;
        _eventHandler = eventHandler;
        _lockManager = lockManager;
        _telemetry = telemetry;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }
    public async Task<WorkflowData> StartWorkflowAsync(Guid workflowId, WorkflowModuleData moduleData, Dictionary<string, object>? initialVariables = null)
    {

        var workflow = await _workflowRepository.GetByIdAsync(workflowId);
        if (workflow == null)
            throw new WorkflowNotFoundException(workflowId);

        var instance = new WorkflowData
        {
            Id = Guid.NewGuid(),
            WorkflowId = workflowId,
            WorkflowVersion = workflow.Version,
            Status = WorkflowStatus.Active,
            CurrentStepId = workflow.InitialStepId,
            ModuleData =moduleData,
            Variables = initialVariables ?? new Dictionary<string, object>(),
            StartedAt = DateTime.UtcNow,
            CreatedAt= DateTime.UtcNow,
            CreatedBy = "",
            UpdatedAt = null,
            UpdatedBy = "",
            IsDeleted = false,
            ErrorDetails = "",


        };

        await _instanceRepository.CreateAsync(instance);
        await _telemetry.TrackWorkflowStarted(instance);
        if (_eventHandler != null)
            await _eventHandler.OnWorkflowStartedAsync(instance);

        await ExecuteStepAsync(instance.Id, workflow.InitialStepId);


        return instance;
    }
    public async Task HandleApprovalTimeoutAsync(Guid workflowId, Guid stepId)
    {
        var instance = await GetInstanceAsync(workflowId);
        if (instance == null) return;

        // Get the workflow definition
        var workflow = await _workflowRepository.GetByIdAsync(instance.WorkflowId);
        var step = workflow.Steps.FirstOrDefault(s => s.Id == stepId);

        if (step == null) return;

        // Get the approval configuration
        var config = step.ActionConfiguration.Deserialize<ApprovalActionConfiguration>();

        // Defensive: config could be null if deserialization failed
        if (config != null && !string.IsNullOrEmpty(config.TimeoutTransition))
        {
            // Transition to timeout step
            var timeoutStep = workflow.Steps.FirstOrDefault(s => s.Name == config.TimeoutTransition);
            if (timeoutStep != null)
            {
                instance.CurrentStepId = timeoutStep.Id;
                await _instanceRepository.UpdateInstanceAsync(instance);

                // Resume workflow execution
                await ResumeWorkflowAsync(workflowId);
            }
        }
        else
        {
            // Default behavior: fail the workflow
            await CancelWorkflowAsync(workflowId);
        }
    }

    public async Task<StepExecutionResult> ExecuteStepAsync(Guid instanceId, Guid stepId)
    {

        using var scope = _serviceProvider.CreateScope();
        WorkflowData? instance = null;
        try
        {
            instance = await _instanceRepository.GetByIdAsync(instanceId);
            var workflow = await _workflowRepository.GetByIdAsync(instance!.WorkflowId);
            var step = workflow.Steps.First(s => s.Id == stepId);

            // Parallel logic
            if (step.IsParallel && step.ParallelSteps != null && step.ParallelSteps.Count > 0)
            {
                var parallelResult = await ExecuteParallelStepsAsync(instance, step);
                // After all parallel steps, continue as normal (could add join logic here)
                return parallelResult;
            }

            // Create execution context
            var context = new WorkflowExecutionContext(instance.WorkflowId, instanceId, _serviceProvider)
            {
                CurrentStepId = stepId,
                ModuleData = instance.ModuleData ?? new WorkflowModuleData(),
                Variables = instance.Variables,
                Status = instance.Status,
                Metadata= workflow.Metadata
            };

            // Update step status
            await UpdateStepStatus(instance, stepId, StepStatus.InProgress);
            await _telemetry.TrackStepStarted(instance, stepId);

            // Get the appropriate action
            var action = _actionResolver.ResolveAction(step.ActionType, scope);
            var actionContext = CreateActionContext(instance, step, context);

            // Execute the action
            if (_eventHandler != null)
                await _eventHandler.OnStepStartedAsync(instanceId, stepId);
            var result = await ExecuteActionWithRetryAsync(action, actionContext, step.RetryPolicy);

            if (result.Success)
            {
                // Update instance variables with action outputs
                foreach (var (key, value) in result.OutputVariables)
                {
                    instance.Variables[key] = value;
                }

                // Check if this is an approval action that needs to pause the workflow
                if (step.ActionType == "Approval" && result.OutputVariables.ContainsKey("status")
                    && result.OutputVariables["status"].ToString() == "pending")
                {
                    // Update workflow state for approval
                    instance.Status = WorkflowStatus.Suspended;
                    instance.Variables["approvalId"] = result.OutputVariables["approvalId"];
                    instance.Variables["approvalStep"] = stepId;

                    await UpdateStepStatus(instance, stepId, StepStatus.Pending);
                    await _instanceRepository.UpdateAsync(instance);

                    return new StepExecutionResult
                    {
                        Success = true,
                        Message = "Workflow paused for approval",
                        Status = StepStatus.Pending,
                        NextStepId = stepId
                    };
                }

                // Normal completion flow
                await UpdateStepStatus(instance, stepId, StepStatus.Completed);
                await _telemetry.TrackStepCompleted(instance, stepId, DateTime.UtcNow - context.StartTime, true);
                if (_eventHandler != null)
                    await _eventHandler.OnStepCompletedAsync(instanceId, stepId, new StepExecutionResult {
                        Success = result.Success,
                        Message = result.Message ?? string.Empty,
                        OutputVariables = result.OutputVariables,
                        Status = StepStatus.Completed,
                        ExecutionTime = DateTime.UtcNow - context.StartTime
                    });

                // Evaluate transitions
                var nextStep = await EvaluateTransitionsAsync(step, instance);
                if (nextStep != null)
                {
                    instance.CurrentStepId = nextStep.Id;
                    instance.Status = WorkflowStatus.Active;

                    // Start next step execution asynchronously
                    _ = Task.Run(() => ExecuteStepAsync(instance.Id, nextStep.Id));
                }
                else
                {
                    // No more steps - complete workflow
                    await CompleteWorkflowAsync(instance, DateTime.UtcNow - instance.StartedAt);
                    if (_eventHandler != null)
                        await _eventHandler.OnWorkflowCompletedAsync(instance);
                }

                await _instanceRepository.UpdateAsync(instance);

                return new StepExecutionResult
                {
                    Success = true,
                    Message = result.Message ?? string.Empty,
                    NextStepId = instance.CurrentStepId,
                    OutputVariables = result.OutputVariables,
                    Status = StepStatus.Completed,
                    ExecutionTime = DateTime.UtcNow - context.StartTime
                };
            }
            else
            {
                // Handle failure
                await HandleStepFailure(instance, step, result.Exception ?? new Exception(result.Message ?? "Step failed"));
                await _telemetry.TrackStepCompleted(instance, stepId, DateTime.UtcNow - context.StartTime, false);
                await _instanceRepository.UpdateAsync(instance);
                if (_eventHandler != null)
                    await _eventHandler.OnStepErrorAsync(instanceId, stepId, result.Exception ?? new Exception(result.Message ?? "Step failed"));

                return new StepExecutionResult
                {
                    Success = false,
                    Message = result.Message ?? string.Empty,
                    Error = result.Exception ?? new Exception(result.Message ?? "Step failed"),
                    Status = StepStatus.Failed,
                    ExecutionTime = DateTime.UtcNow - context.StartTime
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing step {StepId} for workflow instance {InstanceId}",
                stepId, instanceId);

            if (instance == null)
                instance = await _instanceRepository.GetByIdAsync(instanceId);
            if (instance != null)
            {
                var step = (await _workflowRepository.GetByIdAsync(instance.WorkflowId))
                    .Steps.First(s => s.Id == stepId);
                await HandleStepFailure(instance, step, ex);
                await _telemetry.TrackStepError(instance, stepId, ex);
                await _instanceRepository.UpdateAsync(instance);
            }
            if (_eventHandler != null && instance != null)
                await _eventHandler.OnStepErrorAsync(instanceId, stepId, ex);

            throw new WorkflowExecutionException(
                $"Step execution failed: {ex.Message}",
                instance?.WorkflowId ?? Guid.Empty,
                instanceId,
                stepId);
        }
    }

    public async Task<WorkflowStatus> GetWorkflowStatusAsync(Guid instanceId)
    {
        var instance = await _instanceRepository.GetByIdAsync(instanceId);
        if (instance == null)
            throw new WorkflowNotFoundException(instanceId);

        return instance.Status;
    }

    public async Task PauseWorkflowAsync(Guid instanceId)
    {
        var instance = await _instanceRepository.GetByIdAsync(instanceId);
        if (instance == null)
            throw new WorkflowNotFoundException(instanceId);

        if (instance.Status != WorkflowStatus.Active)
            throw new WorkflowException($"Cannot pause workflow in {instance.Status} status");

        instance.Status = WorkflowStatus.Suspended;
        instance.UpdatedAt = DateTime.UtcNow;
        await _instanceRepository.UpdateAsync(instance);
        //await _telemetry.TrackWorkflowPaused(instance);
    }

    public async Task ResumeWorkflowAsync(Guid instanceId)
    {
        var instance = await _instanceRepository.GetByIdAsync(instanceId);
        if (instance == null)
            throw new WorkflowNotFoundException(instanceId);

        if (instance.Status != WorkflowStatus.Suspended)
            throw new WorkflowException($"Cannot resume workflow in {instance.Status} status");

        instance.Status = WorkflowStatus.Active;
        instance.UpdatedAt = DateTime.UtcNow;
        await _instanceRepository.UpdateAsync(instance);
       // await _telemetry.TrackWorkflowResumed(instance);

        // Resume execution from current step
        _ = Task.Run(() => ExecuteStepAsync(instance.Id, instance.CurrentStepId));
    }

    public async Task CancelWorkflowAsync(Guid instanceId)
    {
        var instance = await _instanceRepository.GetByIdAsync(instanceId);
        if (instance == null)
            throw new WorkflowNotFoundException(instanceId);

        if (instance.Status == WorkflowStatus.Completed || instance.Status == WorkflowStatus.Terminated)
            throw new WorkflowException($"Cannot cancel workflow in {instance.Status} status");

        instance.Status = WorkflowStatus.Terminated;
        instance.CompletedAt = DateTime.UtcNow;
        instance.UpdatedAt = DateTime.UtcNow;
        await _instanceRepository.UpdateAsync(instance);
        //await _telemetry.TrackWorkflowCancelled(instance);
    }

    public async Task<WorkflowData> GetInstanceAsync(Guid instanceId)
    {
        var instance = await _instanceRepository.GetByIdAsync(instanceId);
        if (instance == null)
            throw new WorkflowNotFoundException(instanceId);

        return instance;
    }

    public async Task<IEnumerable<WorkflowData>> GetActiveInstancesAsync()
    {
        return await _instanceRepository.GetActiveInstancesAsync();
    }

    public async Task RetryStepAsync(Guid instanceId, Guid stepId)
    {
        var instance = await _instanceRepository.GetByIdAsync(instanceId);
        if (instance == null)
            throw new WorkflowNotFoundException(instanceId);

        var stepInstance = instance.StepInstances.FirstOrDefault(s => s.StepId == stepId);
        if (stepInstance == null)
            throw new WorkflowException($"Step {stepId} not found in workflow instance");

        if (stepInstance.Status != StepStatus.Failed)
            throw new WorkflowException($"Cannot retry step in {stepInstance.Status} status");

        await UpdateStepStatus(instance, stepId, StepStatus.Pending);
        instance.Status = WorkflowStatus.Active;
        await _instanceRepository.UpdateAsync(instance);

        // Retry the step
        _ = Task.Run(() => ExecuteStepAsync(instance.Id, stepId));
    }

    public async Task SkipStepAsync(Guid instanceId, Guid stepId)
    {
        var instance = await _instanceRepository.GetByIdAsync(instanceId);
        if (instance == null)
            throw new WorkflowNotFoundException(instanceId);

        var workflow = await _workflowRepository.GetByIdAsync(instance.WorkflowId);
        var currentStep = workflow.Steps.First(s => s.Id == stepId);

        await UpdateStepStatus(instance, stepId, StepStatus.Skipped);

        // Find next step using transitions
        var nextStep = await EvaluateTransitionsAsync(currentStep, instance);
        if (nextStep != null)
        {
            instance.CurrentStepId = nextStep.Id;
            await _instanceRepository.UpdateAsync(instance);

            // Execute next step
            _ = Task.Run(() => ExecuteStepAsync(instance.Id, nextStep.Id));
        }
        else
        {
            await CompleteWorkflowAsync(instance, DateTime.UtcNow - instance.StartedAt);
        }
    }

    public async Task RollbackStepAsync(Guid instanceId, Guid stepId)
    {
        var instance = await _instanceRepository.GetByIdAsync(instanceId);
        if (instance == null)
            throw new WorkflowNotFoundException(instanceId);

        var stepInstance = instance.StepInstances.FirstOrDefault(s => s.StepId == stepId);
        if (stepInstance == null)
            throw new WorkflowException($"Step {stepId} not found in workflow instance");

        // Find the previous step from history
        var previousStepInstance = instance.StepInstances
            .Where(s => s.CompletedAt < stepInstance.StartedAt)
            .OrderByDescending(s => s.CompletedAt)
            .FirstOrDefault();

        if (previousStepInstance == null)
            throw new WorkflowException("No previous step found to rollback to");

        // Reset current step status
        await UpdateStepStatus(instance, stepId, StepStatus.Pending);

        // Set instance to previous step
        instance.CurrentStepId = previousStepInstance.StepId;
        instance.Status = WorkflowStatus.Active;
        await _instanceRepository.UpdateAsync(instance);

        // Execute from previous step
        _ = Task.Run(() => ExecuteStepAsync(instance.Id, previousStepInstance.StepId));
    }

    public async Task<IEnumerable<StepExecutionHistory>> GetStepHistoryAsync(Guid instanceId)
    {
        var instance = await _instanceRepository.GetByIdAsync(instanceId);
        if (instance == null)
            throw new WorkflowNotFoundException(instanceId);

        var workflow = await _workflowRepository.GetByIdAsync(instance.WorkflowId);

        return instance.StepInstances.Select(si => new StepExecutionHistory
        {
            StepId = si.StepId,
            StepName = workflow.Steps.First(s => s.Id == si.StepId).Name,
            Status = si.Status,
            StartedAt = si.StartedAt,
            CompletedAt = si.CompletedAt,
            RetryCount = si.RetryCount,
            ErrorDetails = si.ErrorDetails,
            InputData = si.InputData,
            OutputData = si.OutputData
        }).ToList();
    }
    private async Task<StepExecutionResult> ExecuteParallelStepsAsync(WorkflowData instance, WorkflowStep step)
    {
        var tasks = step.ParallelSteps.Select(parallelStepId =>
            ExecuteStepAsync(instance.Id, parallelStepId)
        ).ToList();

        await Task.WhenAll(tasks);

        // Check if all parallel steps completed successfully
        var allSuccessful = tasks.All(t => t.Result.Success);
        return new StepExecutionResult
        {
            Success = allSuccessful,
            Message = allSuccessful ? "All parallel steps completed" : "One or more parallel steps failed"
        };
    }

    private async Task<ActionResult> ExecuteActionWithRetryAsync(
        IWorkflowAction action,
        ActionContext context,
        RetryPolicy retryPolicy)
    {
        var attempt = 0;
        var delay = retryPolicy.RetryInterval;

        while (true)
        {
            try
            {
                attempt++;
                return await action.ExecuteAsync(context);
            }
            catch (Exception ex) when (ShouldRetry(ex, attempt, retryPolicy))
            {
                _logger.LogWarning(ex, $"Action execution attempt {attempt} failed, retrying...");
                await Task.Delay(delay);
                if (retryPolicy.ExponentialBackoff)
                {
                    delay *= 2;
                }
            }
        }
    }

    private bool ShouldRetry(Exception ex, int attempt, RetryPolicy policy)
    {
        if (attempt >= policy.MaxRetries)
            return false;

        return policy.RetryableExceptions.Any(exceptionType =>
            ex.GetType().Name.Equals(exceptionType, StringComparison.OrdinalIgnoreCase));
    }

    private async Task<WorkflowStep?> EvaluateTransitionsAsync(WorkflowStep currentStep, WorkflowData instance)
    {
        var workflow = await _workflowRepository.GetByIdAsync(instance.WorkflowId);
        foreach (var transition in currentStep.Transitions.OrderBy(t => t.Priority))
        {
            if (string.IsNullOrEmpty(transition.Condition) ||
                await _expressionEvaluator.EvaluateAsync<bool>(transition.Condition, instance.ModuleData ?? new WorkflowModuleData(), instance.Variables))
            {
                return workflow.Steps.First(s => s.Id == transition.TargetStepId);
            }
        }
        // null is valid here; caller handles null
        return null;
    }

    private async Task HandleStepFailure(WorkflowData instance, WorkflowStep step, Exception exception)
    {
        await UpdateStepStatus(instance, step.Id, StepStatus.Failed);
        instance.Status = WorkflowStatus.Failed;
        instance.ErrorDetails = exception?.ToString() ?? string.Empty;
        await _telemetry.TrackWorkflowError(instance, exception ?? new Exception("Unknown error"));
        if (_eventHandler != null)
            await _eventHandler.OnWorkflowErrorAsync(new WorkflowEngineException(exception?.Message ?? "Workflow error", instance.WorkflowId, instance.Id, step.Id, exception ?? new Exception("Unknown error")));
    }

    private async Task CompleteWorkflowAsync(WorkflowData instance, TimeSpan duration)
    {
        instance.Status = WorkflowStatus.Completed;
        instance.CompletedAt = DateTime.UtcNow;
        await _telemetry.TrackWorkflowCompleted(instance, duration);
        if (_eventHandler != null)
            await _eventHandler.OnWorkflowCompletedAsync(instance);
    }

    private ActionContext CreateActionContext(WorkflowData instance, WorkflowStep step, WorkflowExecutionContext context)
    {
        return new ActionContext
        {
            WorkflowDataId = instance.Id,
            StepId = step.Id,
            ModuleData =instance.ModuleData,
            ActionConfiguration = step.ActionConfiguration,
            Variables = instance.Variables,
            ServiceProvider = _serviceProvider,
            CancellationToken = CancellationToken.None,
            WorkflowExecutionContext = context
        };
    }

    private async Task UpdateStepStatus(WorkflowData instance, Guid stepId, StepStatus status)
    {
        var stepInstance = instance.StepInstances.FirstOrDefault(s => s.StepId == stepId);
        if (stepInstance == null)
        {
            stepInstance = new WorkflowStepData
            {
                StepId = stepId,
                StartedAt = DateTime.UtcNow
            };
            instance.StepInstances.Add(stepInstance);
        }

        stepInstance.Status = status;
        if (status == StepStatus.Completed || status == StepStatus.Failed)
        {
            stepInstance.CompletedAt = DateTime.UtcNow;
        }

        //await _telemetry.TrackStepStatusChanged(instance, stepInstance);
        await Task.CompletedTask; // Ensure method is properly async
    }
}