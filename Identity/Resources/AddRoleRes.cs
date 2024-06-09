using AppIdentity.Domain;

namespace AppIdentity.Resources;

public record AddRoleRes
{
    public string Name { get; set; }
    public int? ModuleId { get; set; }
    public int? SourceId { get; set; }
    public List<KeyValuePair<string, string>> ExtraInfo { get; set; }
    public string NormalizedName { get; set; }
   
    public Translatable DisplayName { get; set; }
}

