namespace AppIdentity.Resources;

public record AddAppUserRoleRes
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public Guid? ModuleId { get; set; }
}