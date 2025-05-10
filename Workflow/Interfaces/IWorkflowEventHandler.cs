namespace AppWorkflow.Core.Interfaces.Services;

using AppWorkflow.Common.Exceptions;
using AppWorkflow.Core.Domain.Data;
using AppWorkflow.Engine;
using System.Text;
using AppWorkflow.Core.DTOs;
using AppWorkflow.Services;
using AppWorkflow.Common.Enums;

public interface IWorkflowEventHandler
{
    Task OnWorkflowStartedAsync(WorkflowData instance);
    Task OnWorkflowCompletedAsync(WorkflowData instance);
    Task OnStepStartedAsync(Guid instanceId, Guid stepId);
    Task OnStepCompletedAsync(Guid instanceId, Guid stepId, StepExecutionResult result);
    Task OnWorkflowErrorAsync(WorkflowEngineException exception);
    Task OnStepErrorAsync(Guid instanceId, Guid stepId, Exception exception);
}

public class WorkflowEventHandler : IWorkflowEventHandler
{
    private readonly IAuditLogService _auditLogService;

    public WorkflowEventHandler(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    public async Task OnWorkflowStartedAsync(WorkflowData instance)
    {
        await _auditLogService.LogAsync(new AuditLogEntry
        {
            EntityType = "Workflow",
            EntityId = instance.Id,
            Action = AuditAction.WorkflowStarted,
            UserId = "system",
            UserName = "system",
            Metadata = new Dictionary<string, string> { { "WorkflowVersion", instance.WorkflowVersion ?? string.Empty } },
            Notes = $"Workflow started."
        });
    }

    public async Task OnWorkflowCompletedAsync(WorkflowData instance)
    {
        await _auditLogService.LogAsync(new AuditLogEntry
        {
            EntityType = "Workflow",
            EntityId = instance.Id,
            Action = AuditAction.WorkflowCompleted,
            UserId = "system",
            UserName = "system",
            Metadata = new Dictionary<string, string> { { "WorkflowVersion", instance.WorkflowVersion ?? string.Empty } },
            Notes = $"Workflow completed."
        });
    }

    public async Task OnStepStartedAsync(Guid instanceId, Guid stepId)
    {
        await _auditLogService.LogAsync(new AuditLogEntry
        {
            EntityType = "WorkflowStep",
            EntityId = stepId,
            Action = AuditAction.StatusChanged,
            UserId = "system",
            UserName = "system",
            Metadata = new Dictionary<string, string> { { "WorkflowInstanceId", instanceId.ToString() } },
            Notes = $"Step started."
        });
    }

    public async Task OnStepCompletedAsync(Guid instanceId, Guid stepId, StepExecutionResult result)
    {
        await _auditLogService.LogAsync(new AuditLogEntry
        {
            EntityType = "WorkflowStep",
            EntityId = stepId,
            Action = AuditAction.StepCompleted,
            UserId = "system",
            UserName = "system",
            Metadata = new Dictionary<string, string> {
                { "WorkflowInstanceId", instanceId.ToString() },
                { "Success", result.Success.ToString() },
                { "Message", result.Message ?? string.Empty }
            },
            Notes = $"Step completed."
        });
    }

    public async Task OnWorkflowErrorAsync(WorkflowEngineException exception)
    {
        await _auditLogService.LogAsync(new AuditLogEntry
        {
            EntityType = "Workflow",
            EntityId = exception.InstanceId ?? Guid.Empty,
            Action = AuditAction.Error,
            UserId = "system",
            UserName = "system",
            Metadata = new Dictionary<string, string> {
                { "WorkflowId", exception.WorkflowId.ToString() },
                { "StepId", exception.StepId?.ToString() ?? string.Empty }
            },
            Notes = $"Workflow error: {exception.Message}"
        });
    }

    public async Task OnStepErrorAsync(Guid instanceId, Guid stepId, Exception exception)
    {
        await _auditLogService.LogAsync(new AuditLogEntry
        {
            EntityType = "WorkflowStep",
            EntityId = stepId,
            Action = AuditAction.Error,
            UserId = "system",
            UserName = "system",
            Metadata = new Dictionary<string, string> { { "WorkflowInstanceId", instanceId.ToString() } },
            Notes = $"Step error: {exception.Message}"
        });
    }
}