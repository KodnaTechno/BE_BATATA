using AppWorkflow.Common.Enums;

using AppWorkflow.Engine;
using AppWorkflow.Infrastructure.Data.Context;
using Microsoft.Extensions.Logging;

namespace AppWorkflow.Infrastructure.Services.Actions;

public class TransactionalStepExecutor : IStepExecutor
{
    private readonly IStepTransactionManager _transactionManager;
    private readonly IWorkflowAction _action;
    private readonly ILogger<TransactionalStepExecutor> _logger;

    public TransactionalStepExecutor(
        IStepTransactionManager transactionManager,
        ILogger<TransactionalStepExecutor> logger)
    {
        _transactionManager = transactionManager;
        _logger = logger;
    }

    public async Task<StepExecutionResult> ExecuteAsync(WorkflowExecutionContext context, WorkflowStep step)
    {
        try
        {
            await _transactionManager.BeginTransactionAsync(context.InstanceId);
            var result = await ExecuteStepInternalAsync(context, step);

            if (result.Success)
            {
                await _transactionManager.CommitTransactionAsync(context.InstanceId);
            }
            else
            {
                await _transactionManager.RollbackTransactionAsync(context.InstanceId);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing step {StepId} for workflow instance {InstanceId}",
                step.Id, context.InstanceId);

            await _transactionManager.RollbackTransactionAsync(context.InstanceId);

            return new StepExecutionResult
            {
                Success = false,
                Message = $"Step execution failed: {ex.Message}",
                Error = ex,
                Status = StepStatus.Failed
            };
        }
    }

    private async Task<StepExecutionResult> ExecuteStepInternalAsync(WorkflowExecutionContext context, WorkflowStep step)
    {
        try
        {
            var actionContext = new ActionContext
            {
                WorkflowDataId = context.InstanceId,
                StepId = step.Id,
                ModuleData = context.ModuleData,
                ActionConfiguration = step.ActionConfiguration,
                Variables = context.Variables,
                ServiceProvider = context.ServiceProvider,
                CancellationToken = context.CancellationToken
            };

            var actionResult = await _action.ExecuteAsync(actionContext);

            return new StepExecutionResult
            {
                Success = actionResult.Success,
                Message = actionResult.Message,
                OutputVariables = actionResult.OutputVariables,
                Status = actionResult.Success ? StepStatus.Completed : StepStatus.Failed,
                ExecutionTime = DateTime.UtcNow - context.StartTime
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Internal error executing step {StepId} for workflow instance {InstanceId}",
                step.Id, context.InstanceId);

            throw; // Rethrow to be handled by outer try-catch
        }
    }

    public async Task<bool> ValidateAsync(WorkflowStep step)
    {
        try
        {
            return (await _action.ValidateConfigurationAsync(step.ActionConfiguration)).IsValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating step {StepId}", step.Id);
            return false;
        }
    }

    public async Task<object> GetConfigurationSchemaAsync()
    {
        return await _action.GetConfigurationSchemaAsync();
    }
}