using Application.Features.ControlPanel.Workspace.Commands;
using Application.Features.ControlPanel.Workspace.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InitController : ControllerBase
    {
        private readonly IStringLocalizer<object> _localization;
        private readonly ILogger<InitController> _logger;
        private readonly IMediator _mediator;

        public InitController(ILogger<InitController> logger, IStringLocalizer<object> localization, IMediator mediator)
        {
            _logger = logger;
            _localization = localization;
            _mediator = mediator;
        }

        //[HttpGet]
        //public IActionResult Get()
        //{
        //    var welcomeMessage = _localization["Welcome"].Value;
        //    return Ok(welcomeMessage);
        //}

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateWorkspaceCommand command)
            => Ok(await _mediator.Send(command));

        [HttpGet("{WorkspaceId}")]
        public async Task<IActionResult> GetW([FromRoute] GetWorkspaceQuery query)
          => throw new NotImplementedException();

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] GetWorkspacesQuery query)
         => Ok(await _mediator.Send(query));




    }
}
