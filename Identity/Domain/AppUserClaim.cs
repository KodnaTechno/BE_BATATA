using Microsoft.AspNetCore.Identity;

namespace AppIdentity.Domain;

public class AppUserClaim : IdentityUserClaim<Guid>
{
    public string Username { get; set; }
    public string ClaimValueRef { get; set; }
}