using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;

namespace AppIdentity.Domain;

public class AppUserRole : IdentityUserRole<string>
{
    public int? ModuleId { get; set; }
}