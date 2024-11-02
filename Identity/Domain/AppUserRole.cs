using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;

namespace AppIdentity.Domain;

public class AppUserRole : IdentityUserRole<Guid>
{
    public Guid? ModuleId { get; set; }
}