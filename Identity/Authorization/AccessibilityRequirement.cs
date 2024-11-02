using AppIdentity.Database;
using AppIdentity.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace AppIdentity.Authorization;

public class AccessibilityRequirement : IAuthorizationRequirement
{
    public string Module { get; }
    public AccessibilityRequirement(string module) => Module = module;
}

public class AccessibilityRequirementHandler : AuthorizationHandler<AccessibilityRequirement>
{
    private readonly AppIdentityDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserProvider _userProvider;
    private readonly IAccessibilityProvider _AccessibilityProvider;

    public AccessibilityRequirementHandler(IUserProvider userProvider, AppIdentityDbContext dbContext, IHttpContextAccessor httpContextAccessor, IAccessibilityProvider accessibilityProvider)
    {
        _dbContext = dbContext;
        _AccessibilityProvider = accessibilityProvider;
        _userProvider = userProvider;
    }
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        AccessibilityRequirement requirement)
    {
        if (!_userProvider.IsAuthenticated)
        {
            return Task.CompletedTask;
        }

        var modules = requirement.Module.Split(',');
        var currentUser = _userProvider.CurrentUser;

        foreach (var module in modules)
        {
            if (module == "ignore")
            {
                foreach (var req in context.PendingRequirements)
                {
                    context.Succeed(req);
                }
                break;
            }

            if (module == "ignoreOthers")
            {
                foreach (var pendingReq in context.PendingRequirements)
                {
                    if (pendingReq != requirement)
                    {
                        context.Succeed(pendingReq);
                    }
                }
                continue;
            }

            var users = _AccessibilityProvider.GetAccessibilityUsers(module);

            if (users is null) continue;

            var isUserInGroup = users.Any(x => x.Id == currentUser.Id);

            if (isUserInGroup)
            {
                context.Succeed(requirement);
                break;
            }
        }

        return Task.CompletedTask;
    }
}
