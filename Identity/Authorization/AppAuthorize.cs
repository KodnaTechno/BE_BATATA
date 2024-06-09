using System.IdentityModel.Tokens.Jwt;
using System.Net;
using AppIdentity.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace AppIdentity.Authorization;

public class AppAuthorize : AuthorizeAttribute, IAuthorizationFilter
{
    private IUserProvider UserProvider;
    public AppAuthorize()
    {
    }
    public void OnAuthorization(AuthorizationFilterContext filterContext)
    {
      
    }
  
}

class Result : IActionResult
{
    public string Status { get; set; }
    public string Message { get; set; }
    public async Task ExecuteResultAsync(ActionContext context)
    {
    }
}
