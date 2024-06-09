using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace AppIdentity.Authorization;

public class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    const string POLICY_PREFIX = "Permission";
    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
    {
        return Task.FromResult(
            new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser().Build());
    }

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
    {
        return Task.FromResult<AuthorizationPolicy?>(null);
    }

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith(POLICY_PREFIX, StringComparison.OrdinalIgnoreCase))
        {
            var policy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);
            policy.AddRequirements(new AccessibilityRequirement(policyName.Substring(POLICY_PREFIX.Length)));
            return Task.FromResult(policy.Build());
        }
        else
        {
            return Task.FromResult<AuthorizationPolicy?>(null);
        }
    }
}
