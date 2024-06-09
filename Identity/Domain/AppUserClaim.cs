using Microsoft.AspNetCore.Identity;

namespace AppIdentity.Domain;

public class AppUserClaim : IdentityUserClaim<string>
{
    public string Username { get; set; }
    public string ClaimValueRef { get; set; }
}