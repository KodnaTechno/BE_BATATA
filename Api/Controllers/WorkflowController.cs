using Application.Features.WorkFlow.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Core.DTOs;
using AppWorkflow.Engine;
using AppWorkflow.Services;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/workflow")]
    public class WorkflowController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IWorkflowEngine _workflowEngine;
        private readonly IAuditLogService _auditLogService;
        private readonly AppWorkflow.Triggers.TriggerManager _triggerManager;

        public WorkflowController(IMediator mediator, IWorkflowEngine workflowEngine, IAuditLogService auditLogService, AppWorkflow.Triggers.TriggerManager triggerManager)
        {
            _mediator = mediator;
            _workflowEngine = workflowEngine;
            _auditLogService = auditLogService;
            _triggerManager = triggerManager;
        }

        [HttpPost("execute")]
        public async Task<IActionResult> ExecuteWorkflow([FromBody] WorkflowCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("status/{workflowId}")]
        public async Task<IActionResult> GetWorkflowStatus(Guid workflowId)
        {
            // Endpoint to check status of asynchronous workflows
            var status = await _workflowEngine.GetWorkflowStatusAsync(workflowId);
            return Ok(status);
        }

        [HttpGet("{instanceId}")]
        public async Task<IActionResult> GetWorkflowInstance(Guid instanceId)
        {
            var instance = await _workflowEngine.GetInstanceAsync(instanceId);
            return Ok(instance);
        }

        [HttpGet("{instanceId}/steps")]
        public async Task<IActionResult> GetStepHistory(Guid instanceId)
        {
            var steps = await _workflowEngine.GetStepHistoryAsync(instanceId);
            return Ok(steps);
        }

        [HttpGet("audit")]
        public async Task<IActionResult> GetAuditLogs([FromQuery] AuditLogFilter filter)
        {
            var logs = await _auditLogService.SearchAsync(filter);
            return Ok(logs);
        }

        [HttpGet("instances")]
        public async Task<IActionResult> GetAllInstances()
        {
            var instances = await _workflowEngine.GetActiveInstancesAsync();
            return Ok(instances);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveInstances()
        {
            var active = await _workflowEngine.GetActiveInstancesAsync();
            return Ok(active);
        }

        [HttpGet("{instanceId}/audit")]
        public async Task<IActionResult> GetAuditLogsForInstance(Guid instanceId)
        {
            var filter = new AuditLogFilter { EntityId = instanceId, EntityType = "Workflow" };
            var logs = await _auditLogService.SearchAsync(filter);
            return Ok(logs);
        }

        [HttpGet("audit/{auditLogId}")]
        public async Task<IActionResult> GetAuditLogById(Guid auditLogId)
        {
            // This assumes you have a method to get a single audit log by ID
            var filter = new AuditLogFilter { Skip = 0, Take = 1 };
            var logs = await _auditLogService.SearchAsync(filter);
            var log = logs.FirstOrDefault(l => l.Id == auditLogId);
            if (log == null) return NotFound();
            return Ok(log);
        }

        [HttpPost("{instanceId}/cancel")]
        public async Task<IActionResult> CancelWorkflow(Guid instanceId)
        {
            await _workflowEngine.CancelWorkflowAsync(instanceId);
            return NoContent();
        }

        [HttpPost("{instanceId}/pause")]
        public async Task<IActionResult> PauseWorkflow(Guid instanceId)
        {
            await _workflowEngine.PauseWorkflowAsync(instanceId);
            return NoContent();
        }

        [HttpPost("{instanceId}/resume")]
        public async Task<IActionResult> ResumeWorkflow(Guid instanceId)
        {
            await _workflowEngine.ResumeWorkflowAsync(instanceId);
            return NoContent();
        }

        [HttpPost("{instanceId}/retry/{stepId}")]
        public async Task<IActionResult> RetryStep(Guid instanceId, Guid stepId)
        {
            await _workflowEngine.RetryStepAsync(instanceId, stepId);
            return NoContent();
        }

        [HttpPost("{instanceId}/skip/{stepId}")]
        public async Task<IActionResult> SkipStep(Guid instanceId, Guid stepId)
        {
            await _workflowEngine.SkipStepAsync(instanceId, stepId);
            return NoContent();
        }

        [HttpPost("{instanceId}/rollback/{stepId}")]
        public async Task<IActionResult> RollbackStep(Guid instanceId, Guid stepId)
        {
            await _workflowEngine.RollbackStepAsync(instanceId, stepId);
            return NoContent();
        }

        [HttpPost("trigger")]
        public async Task<IActionResult> TriggerWorkflow([FromBody] AppWorkflow.Infrastructure.Triggers.TriggerContext context)
        {
            await _triggerManager.HandleTriggerEventAsync(context);
            return Ok(new { Success = true, Message = $"Workflow(s) triggered for type {context.TriggerType}" });
        }
    }
}
