using Application.Features.ControlPanel.Properties.Commands;
using Application.Features.ControlPanel.Properties.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PropertyController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProperty([FromBody] CreatePropertyCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpGet]
        public async Task<IActionResult> GetProperties(
            [FromQuery] Guid? applicationId,
            [FromQuery] Guid? workspaceId,
            [FromQuery] Guid? moduleId,
            [FromQuery] Guid? workspaceModuleId)
        {
            var query = new GetPropertiesQuery
            {
                ApplicationId = applicationId,
                WorkspaceId = workspaceId,
                ModuleId = moduleId,
                WorkspaceModuleId = workspaceModuleId
            };

            return Ok(await _mediator.Send(query));
        }

        [HttpGet("{propertyId}")]
        public async Task<IActionResult> GetProperty([FromRoute] Guid propertyId)
        {
            var query = new GetPropertyQuery { PropertyId = propertyId };
            return Ok(await _mediator.Send(query));
        }

        [HttpPut("{propertyId}")]
        public async Task<IActionResult> UpdateProperty([FromRoute] Guid propertyId, [FromBody] UpdatePropertyCommand command)
        {
            command.PropertyId = propertyId;
            return Ok(await _mediator.Send(command));
        }

        [HttpDelete("{propertyId}")]
        public async Task<IActionResult> DeleteProperty([FromRoute] Guid propertyId)
        {
            var command = new DeletePropertyCommand { PropertyId = propertyId };
            return Ok(await _mediator.Send(command));
        }
    }
}