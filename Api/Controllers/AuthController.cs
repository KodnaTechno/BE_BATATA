using AppCommon.DTOs.Identity;
using AppIdentity.IServices;
using AppIdentity.Resources;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserProvider _userProvider;
        public AuthController(IUserProvider userProvider)
        {
            _userProvider = userProvider;
        }
        // POST api/<AuthController>
        [HttpPost]
        public async Task<ActionResult<LoginResultModel>> Post([FromBody] LoginRequestModel model)
        {
            if ( _userProvider.Authenticate(model.UserName, model.Password) is AuthenticateResult _res && _res.Succeeded)
            {
                
                var userId = _res.AppUser.Id;
                var token = _res.Token;
                return new LoginResultModel { Successful = true, Token = token,UserId= userId };
            }

            return new LoginResultModel { Successful = false, Error = "Username or password is incorrect." };
        }




    }
}
