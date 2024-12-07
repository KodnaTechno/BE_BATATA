namespace AppIdentity.Resources;

public record PermissionRes
{
    public string DisplayName { get; set; }
    public Guid? ModuleId { get; set; }
    public string Command { get; set; }
    public string ModuleType { get; set; }
}

