using Microsoft.AspNetCore.Mvc;
using Module;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertyDataController : ControllerBase
    {

        public PropertyDataController(ModuleDbContext context)
        {
        }

    }
}

