using AppIdentity.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;

namespace AppIdentity.Domain;

public class AppRole : IdentityRole<Guid>
{
    public Guid? ModuleId { get; set; }
    public RoleModulesEnum ModuleType { get; set; }
    public int? SourceId { get; set; }
    public List<KeyValuePair<string, string>> ExtraInfo { get; set; }
    
    public ICollection<AppPermission> Permissions { get; set; }
  
    public Translatable DisplayName { get; set; }
}