using AppCommon.DTOs;
using AppIdentity.Domain;

namespace AppIdentity.Resources;

public record UpdateRoleRes
{
    public string Name { get; set; }
    public Guid? ModuleId { get; set; }
    public int? SourceId { get; set; }
    public TranslatableValue DisplayName { get; set; }

}

