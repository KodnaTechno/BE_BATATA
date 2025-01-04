
using AppWorkflow.Core.Domain.Data;
using AppWorkflow.Core.Domain.Schema;
using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Core.Models;
using AppWorkflow.Engine;
using AppWorkflow.Infrastructure.Repositories.IRepository;
using AppWorkflow.Infrastructure.Services.Actions;

namespace AppWorkflow.Infrastructure.Services.Engine;


public class WorkflowEngine: IWorkflowEngine
    {
        private readonly IActionResolver _actionResolver;
        private readonly IWorkflowRepository _workflowRepository;
        private readonly IWorkflowDataRepository _instanceRepository;
        private readonly IExpressionEvaluator _expressionEvaluator;
        private readonly ILogger<WorkflowEngine> _logger;
        private readonly ITelemetryTracker _telemetry;
        private readonly IServiceProvider _serviceProvider;
        
        public async Task<WorkflowData> StartWorkflowAsync(Guid workflowId, WorkflowModuleData moduleData, Dictionary<string, object> initialVariables = null)
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
                StartedAt = DateTime.UtcNow
            };

            await _instanceRepository.CreateAsync(instance);
            await _telemetry.TrackWorkflowStarted(instance);

            // Start execution of initial step
            _ = Task.Run(() => ExecuteStepAsync(instance.Id, workflow.InitialStepId));

            return instance;
        }

        public async Task<StepExecutionResult> ExecuteStepAsync(Guid instanceId, Guid stepId)
        {

            var instance = await _instanceRepository.GetByIdAsync(instanceId);
            var workflow = await _workflowRepository.GetByIdAsync(instance.WorkflowId);
            var step = workflow.Steps.First(s => s.Id == stepId);

            try
            {
                // Update step status
                await UpdateStepStatus(instance, stepId, StepStatus.InProgress);

                // Handle parallel steps
                if (step.IsParallel)
                {
                    return await ExecuteParallelStepsAsync(instance, step);
                }

                // Execute the action
                var action = _actionResolver.ResolveAction(step.ActionType);
                var context = CreateActionContext(instance, step);
                var result = await ExecuteActionWithRetryAsync(action, context, step.RetryPolicy);

                if (result.Success)
                {
                    // Update instance variables with action outputs
                    foreach (var (key, value) in result.OutputVariables)
                    {
                        instance.Variables[key] = value;
                    }

                    await UpdateStepStatus(instance, stepId, StepStatus.Completed);

                    // Evaluate transitions
                    var nextStep = await EvaluateTransitionsAsync(step, instance);
                    if (nextStep != null)
                    {
                        instance.CurrentStepId = nextStep.Id;
                        // Start next step execution
                        _ = Task.Run(() => ExecuteStepAsync(instance.Id, nextStep.Id));
                    }
                    else
                    {
                        // No more steps - complete workflow
                        await CompleteWorkflowAsync(instance);
                    }
                }
                else
                {
                    await HandleStepFailure(instance, step, result.Exception);
                }

                await _instanceRepository.UpdateAsync(instance);
                return new StepExecutionResult
                {
                    Success = result.Success,
                    Message = result.Message,
                    NextStepId = instance.CurrentStepId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error executing step {stepId}");
                await HandleStepFailure(instance, step, ex);
                throw;
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
            await CompleteWorkflowAsync(instance);
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
            InputData = si.InputData?.Deserialize<Dictionary<string, object>>(),
            OutputData = si.OutputData?.Deserialize<Dictionary<string, object>>()
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

        private async Task<WorkflowStep> EvaluateTransitionsAsync(WorkflowStep currentStep, WorkflowData instance)
        {
            var workflow = await _workflowRepository.GetByIdAsync(instance.WorkflowId);

            foreach (var transition in currentStep.Transitions.OrderBy(t => t.Priority))
            {
                //if (string.IsNullOrEmpty(transition.Condition) ||
                //    await _expressionEvaluator.EvaluateAsync<bool>(
                //        transition.Condition,
                //        instance.ModuleData,
                //        instance.Variables))
                //{
                //    return workflow.Steps.First(s => s.Id == transition.TargetStepId);
                //}
            }

            return null;
        }

        private async Task HandleStepFailure(WorkflowData instance, WorkflowStep step, Exception exception)
        {
            await UpdateStepStatus(instance, step.Id, StepStatus.Failed);
            instance.Status = WorkflowStatus.Failed;
            instance.ErrorDetails = exception?.ToString();
            //await _telemetry.TrackStepFailed(instance, step, exception);
        }

        private async Task CompleteWorkflowAsync(WorkflowData instance)
        {
            instance.Status = WorkflowStatus.Completed;
            instance.CompletedAt = DateTime.UtcNow;
            //await _telemetry.TrackWorkflowCompleted(instance);
        }

        private ActionContext CreateActionContext(WorkflowData instance, WorkflowStep step)
        {
            return new ActionContext
            {
                WorkflowDataId = instance.Id,
                StepId = step.Id,
                ModuleData =instance.ModuleData,
                ActionConfiguration = step.ActionConfiguration,
                Variables = instance.Variables,
                ServiceProvider = _serviceProvider,
                CancellationToken = CancellationToken.None
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
        }
    }