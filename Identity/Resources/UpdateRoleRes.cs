using AppIdentity.Domain;

namespace AppIdentity.Resources;

public record UpdateRoleRes
{
    public string Name { get; set; }
    public int? ModuleId { get; set; }
    public int? SourceId { get; set; }
    public Translatable DisplayName { get; set; }

}

