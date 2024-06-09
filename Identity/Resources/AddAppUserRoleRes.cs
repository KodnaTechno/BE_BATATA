namespace AppIdentity.Resources;

public record AddAppUserRoleRes
{
    public string UserId { get; set; }
    public string RoleId { get; set; }
    public int? ModuleId { get; set; }
}