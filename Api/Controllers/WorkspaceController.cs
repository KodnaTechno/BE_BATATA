using Application.Features.ControlPanel.Workspace.Commands;
using Application.Features.ControlPanel.Workspace.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class WorkspaceController : ControllerBase
    {

        private readonly IMediator _mediator;

        public WorkspaceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{WorkspaceId}")]
        public async Task<IActionResult> Get([FromRoute] GetWorkspaceQuery query)
           => Ok(await _mediator.Send(query));

        [HttpGet("{ApplicationId}")]
        public async Task<IActionResult> GetList([FromRoute] GetWorkspacesQuery query)
           => Ok(await _mediator.Send(query));

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateWorkspaceCommand command)
            => Ok(await _mediator.Send(command));

    }
}
