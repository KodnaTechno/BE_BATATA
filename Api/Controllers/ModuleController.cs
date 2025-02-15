using Application.Features.ControlPanel.Modules.Commands;
using Application.Features.ControlPanel.Modules.Queries;
using Application.Features.ControlPanel.Workspace.Commands;
using Application.Features.ControlPanel.Workspace.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModuleController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet("single/{ModuleId}")]
        public async Task<IActionResult> Get([FromRoute] GetModuleQuery query)
            => Ok(await _mediator.Send(query));

        [HttpGet("list/{ApplicationId?}")]
        public async Task<IActionResult> GetList([FromRoute] GetModulesQuery query)
            => Ok(await _mediator.Send(query));

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateModuleCommand command)
            => Ok(await _mediator.Send(command));
   
    }
}
