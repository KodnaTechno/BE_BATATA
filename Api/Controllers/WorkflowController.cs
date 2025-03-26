using Application.Features.WorkFlow.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/workflow")]
    public class WorkflowController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WorkflowController(IMediator mediator)
        {
            _mediator = mediator;
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
            // Implementation would use your workflow engine
            return Ok();
        }
    }
}
