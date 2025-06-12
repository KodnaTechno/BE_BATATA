using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AppWorkflow.Infrastructure.Repositories.IRepository;
using AppWorkflow.Core.Domain.Data;
using System;
using System.Linq;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkflowHistoryController : ControllerBase
    {
        private readonly IWorkflowDataRepository _instanceRepo;
        private readonly IAuditLogService _auditLogService;

        public WorkflowHistoryController(IWorkflowDataRepository instanceRepo, IAuditLogService auditLogService)
        {
            _instanceRepo = instanceRepo;
            _auditLogService = auditLogService;
        }

        [HttpGet("workflows/{workflowId}/history")]
        public async Task<IActionResult> GetWorkflowHistory(Guid workflowId)
        {
            // Get all instances for a specific workflow
            var all = await _instanceRepo.GetInstancesByStatus(WorkflowStatus.Active);
            var filtered = all?.Where(i => i.WorkflowId == workflowId).ToList();
            return Ok(filtered);
        }

        [HttpGet("workflow-instances/{instanceId}/history")]
        public async Task<IActionResult> GetInstanceHistory(Guid instanceId)
        {
            var instance = await _instanceRepo.GetByIdAsync(instanceId);
            return Ok(instance?.StepInstances);
        }

        [HttpGet("workflow-instances/{instanceId}/steps/{stepId}/log")]
        public async Task<IActionResult> GetStepLog(Guid instanceId, Guid stepId)
        {
            var instance = await _instanceRepo.GetByIdAsync(instanceId);
            var step = instance?.StepInstances?.FirstOrDefault(s => s.StepId == stepId);
            // If you have a Log property, return it; otherwise, return the step details
            return Ok(step?.Log ?? step);
        }

        [HttpGet("audit")]
        public async Task<IActionResult> GetAuditTrail([FromQuery] string userId, [FromQuery] string action, [FromQuery] string from, [FromQuery] string to)
        {
            var filter = new AuditLogFilter { UserId = userId, Action = action, From = from, To = to };
            var logs = await _auditLogService.SearchAsync(filter);
            return Ok(logs);
        }
    }
}
