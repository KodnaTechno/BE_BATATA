using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Text.Json;

namespace AppIdentity.Authorization;

public class AccessibilityPermissionAttribute : AuthorizeAttribute,IAsyncAuthorizationFilter
{
    const string POLICY_PREFIX = "Permission";
    private readonly string _requiredPermission;
    private readonly string _key;
    private string _globalParam;
    private bool HasKey = false;

    public AccessibilityPermissionAttribute(string requiredPermission, string key = null)
    {
        _requiredPermission = POLICY_PREFIX + requiredPermission;
        if (!string.IsNullOrEmpty(key))
        {
            HasKey = true;
            _key = key;
        }
    }
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var httpContext = context.HttpContext;

        if (HasKey && !await CheckPermissionAsync(httpContext, _requiredPermission))
        {
            context.Result = new ForbidResult();
            return;
        }

        var authService = context.HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();
        var policyName = HasKey ? $"{_requiredPermission}_{_globalParam}" : _requiredPermission;
        var auhtorizationResult= await authService.AuthorizeAsync(context.HttpContext.User, null, policyName);

        if (!auhtorizationResult.Succeeded)
            context.Result = new ForbidResult();
    }
    private async Task<bool> CheckPermissionAsync(HttpContext httpContext, string requiredPermission)
    {
        if (httpContext.Request.RouteValues.TryGetValue(_key, out var routeValue))
        {
            _globalParam = routeValue.ToString();
            return true;
        }
        if (httpContext.Request.Query.TryGetValue(_key, out var queryValue))
        {
            _globalParam = queryValue.ToString();
            return true;
        }

        if (httpContext.Request.Body.CanRead)
        {
            httpContext.Request.EnableBuffering();
            httpContext.Request.Body.Position = 0;
            var body = await new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true).ReadToEndAsync();
            dynamic requestBody = JsonSerializer.Deserialize<object>(body);

            if (requestBody != null && requestBody[queryValue] != null)
            {
                _globalParam = requestBody.levelId.ToString();
                return true;
            }
            httpContext.Request.Body.Position = 0;
        }
        return false;
    }

   
}