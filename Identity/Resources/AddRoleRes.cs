using AppIdentity.Domain;
using AppIdentity.Domain.Enums;

namespace AppIdentity.Resources;

public record AddRoleRes
{
    public string Name { get; set; }
    public Guid? ModuleId { get; set; }
    public RoleModulesEnum ModuleType { get; set; }
    public int? SourceId { get; set; }
    public List<KeyValuePair<string, string>> ExtraInfo { get; set; }
    public string NormalizedName { get; set; }
   
    public Translatable DisplayName { get; set; }
}

