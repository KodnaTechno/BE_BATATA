using Application.Features.ControlPanel.Workspace.Commands;
using Application.Features.ControlPanel.Workspace.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkspaceController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet("single/{WorkspaceId}")]
        public async Task<IActionResult> Get([FromRoute] GetWorkspaceQuery query)
            => Ok(await _mediator.Send(query));

        [HttpGet("list/{ApplicationId}")]
        public async Task<IActionResult> GetList([FromRoute] GetWorkspacesQuery query)
            => Ok(await _mediator.Send(query));

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateWorkspaceCommand command)
            => Ok(await _mediator.Send(command));

        [HttpPut("{WorkspaceId}/modules")]
        public async Task<IActionResult> AssignModules(
            [FromRoute] Guid WorkspaceId,
            [FromBody] AssignWorkspaceModulesCommand command)
        {
            command.WorkspaceId = WorkspaceId;
            return Ok(await _mediator.Send(command));
        }

        [HttpGet("{WorkspaceId}/modules")]
        public async Task<IActionResult> GetAssignedModules(
            [FromRoute] Guid WorkspaceId,
            [FromQuery] GetAssignedModulesQuery query)
        {
            query.WorkspaceId = WorkspaceId;
            return Ok(await _mediator.Send(query));
        }
    }
}
