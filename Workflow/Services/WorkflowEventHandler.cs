using AppWorkflow.Common.Enums;
using AppWorkflow.Common.Exceptions;
using AppWorkflow.Core.Domain.Data;
using AppWorkflow.Core.DTOs;
using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Engine;
using AppWorkflow.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace AppWorkflow.Services
{
    public class WorkflowEventHandler : IWorkflowEventHandler
    {
        private readonly IAuditLogService _auditLogService;
        private readonly ILogger<WorkflowEventHandler> _logger;

        public WorkflowEventHandler(IAuditLogService auditLogService, ILogger<WorkflowEventHandler> logger)
        {
            _auditLogService = auditLogService;
            _logger = logger;
        }

        public async Task OnWorkflowStartedAsync(WorkflowData instance)
        {
            _logger.LogInformation(
                "Workflow started: {WorkflowId} (Instance: {InstanceId}, Version: {Version})",
                instance.WorkflowId, instance.Id, instance.WorkflowVersion);

            await _auditLogService.LogAsync(new AuditLogEntry
            {
                EntityType = "Workflow",
                EntityId = instance.Id,
                Action = AuditAction.WorkflowStarted,
                UserId = "system",
                UserName = "system",
                Metadata = new Dictionary<string, string> { 
                    { "WorkflowId", instance.WorkflowId.ToString() },
                    { "WorkflowVersion", instance.WorkflowVersion ?? string.Empty } 
                },
                Notes = $"Workflow started."
            });
        }

        public async Task OnWorkflowCompletedAsync(WorkflowData instance)
        {
            _logger.LogInformation(
                "Workflow completed: {WorkflowId} (Instance: {InstanceId}, Duration: {Duration}ms)",
                instance.WorkflowId, instance.Id, 
                instance.CompletedAt.HasValue 
                    ? (instance.CompletedAt.Value - instance.StartedAt).TotalMilliseconds 
                    : 0);

            await _auditLogService.LogAsync(new AuditLogEntry
            {
                EntityType = "Workflow",
                EntityId = instance.Id,
                Action = AuditAction.WorkflowCompleted,
                UserId = "system",
                UserName = "system",
                Metadata = new Dictionary<string, string> { 
                    { "WorkflowId", instance.WorkflowId.ToString() },
                    { "WorkflowVersion", instance.WorkflowVersion ?? string.Empty },
                    { "Duration", instance.CompletedAt.HasValue 
                        ? (instance.CompletedAt.Value - instance.StartedAt).TotalMilliseconds.ToString() 
                        : "0" }
                },
                Notes = $"Workflow completed."
            });
        }

        public async Task OnStepStartedAsync(Guid instanceId, Guid stepId)
        {
            _logger.LogDebug("Step started: {StepId} (Instance: {InstanceId})", stepId, instanceId);

            await _auditLogService.LogAsync(new AuditLogEntry
            {
                EntityType = "WorkflowStep",
                EntityId = stepId,
                Action = AuditAction.StatusChanged,
                UserId = "system",
                UserName = "system",
                Metadata = new Dictionary<string, string> { 
                    { "WorkflowInstanceId", instanceId.ToString() },
                    { "Status", "Started" }
                },
                Notes = $"Step started."
            });
        }

        public async Task OnStepCompletedAsync(Guid instanceId, Guid stepId, StepExecutionResult result)
        {
            _logger.LogDebug(
                "Step completed: {StepId} (Instance: {InstanceId}, Success: {Success}, Duration: {Duration}ms)",
                stepId, instanceId, result.Success, result.ExecutionTime.TotalMilliseconds);

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
                    { "Message", result.Message ?? string.Empty },
                    { "Duration", result.ExecutionTime.TotalMilliseconds.ToString() }
                },
                Notes = $"Step completed."
            });
        }

        public async Task OnWorkflowErrorAsync(WorkflowEngineException exception)
        {
            _logger.LogError(
                exception,
                "Workflow error: {WorkflowId} (Instance: {InstanceId}, Step: {StepId})",
                exception.WorkflowId, exception.InstanceId, exception.StepId);

            await _auditLogService.LogAsync(new AuditLogEntry
            {
                EntityType = "Workflow",
                EntityId = exception.InstanceId ?? Guid.Empty,
                Action = AuditAction.Error,
                UserId = "system",
                UserName = "system",
                Metadata = new Dictionary<string, string> {
                    { "WorkflowId", exception.WorkflowId.ToString() },
                    { "StepId", exception.StepId?.ToString() ?? string.Empty },
                    { "ErrorType", exception.GetType().Name }
                },
                Notes = $"Workflow error: {exception.Message}"
            });
        }

        public async Task OnStepErrorAsync(Guid instanceId, Guid stepId, Exception exception)
        {
            _logger.LogError(
                exception,
                "Step error: {StepId} (Instance: {InstanceId})",
                stepId, instanceId);

            await _auditLogService.LogAsync(new AuditLogEntry
            {
                EntityType = "WorkflowStep",
                EntityId = stepId,
                Action = AuditAction.Error,
                UserId = "system",
                UserName = "system",
                Metadata = new Dictionary<string, string> { 
                    { "WorkflowInstanceId", instanceId.ToString() },
                    { "ErrorType", exception.GetType().Name }
                },
                Notes = $"Step error: {exception.Message}"
            });
        }
    }
}
