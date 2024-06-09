using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;

namespace AppIdentity.Domain;

public class AppRolePermission : IdentityRoleClaim<string>
{
    public int AppPermissionId { get; set; }
    public AppPermission AppPermission { get; set; }
}