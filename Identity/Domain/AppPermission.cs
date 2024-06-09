using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;

namespace AppIdentity.Domain;

public class AppPermission
{
    public int Id { get; set; }
    public string DisplayName { get; set; }
    public int? ModuleId { get; set; }
    
    public string ModuleType { get; set; }
    public string Command { get; set; }
    public ICollection<AppRolePermission> RolePermissions { get; set; }
    public ICollection<AppGroupPermission> GroupPermissions { get; set; }
}