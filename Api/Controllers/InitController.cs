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

        public InitController(ILogger<InitController> logger, IStringLocalizer<object> localization)
        {
            _logger = logger;
            _localization = localization;
        }

        [HttpGet]
        public IActionResult Get()
        {
            throw new NotImplementedException();

            var welcomeMessage = _localization["Welcome"].Value;
            return Ok(welcomeMessage);
        }
    }
}
