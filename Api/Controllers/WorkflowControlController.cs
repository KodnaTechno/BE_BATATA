using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AppWorkflow.Infrastructure.Services.Engine;
using System;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkflowControlController : ControllerBase
    {
        private readonly IWorkflowEngine _engine;

        public WorkflowControlController(IWorkflowEngine engine)
        {
            _engine = engine;
        }

        [HttpPost("workflow-instances/{instanceId}/pause")]
        public async Task<IActionResult> PauseInstance(Guid instanceId)
        {
            try {
                await _engine.PauseWorkflowAsync(instanceId);
                return NoContent();
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("workflow-instances/{instanceId}/resume")]
        public async Task<IActionResult> ResumeInstance(Guid instanceId)
        {
            try {
                await _engine.ResumeWorkflowAsync(instanceId);
                return NoContent();
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("workflow-instances/{instanceId}/steps/{stepId}/skip")]
        public async Task<IActionResult> SkipStep(Guid instanceId, Guid stepId)
        {
            try {
                await _engine.SkipStepAsync(instanceId, stepId);
                return NoContent();
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("workflow-instances/{instanceId}/steps/{stepId}/retry")]
        public async Task<IActionResult> RetryStep(Guid instanceId, Guid stepId)
        {
            try {
                await _engine.RetryStepAsync(instanceId, stepId);
                return NoContent();
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("workflow-instances/{instanceId}/status")]
        public async Task<IActionResult> GetInstanceStatus(Guid instanceId)
        {
            try {
                var status = await _engine.GetWorkflowStatusAsync(instanceId);
                return Ok(status);
            } catch (Exception ex) {
                return NotFound(ex.Message);
            }
        }
    }
}
