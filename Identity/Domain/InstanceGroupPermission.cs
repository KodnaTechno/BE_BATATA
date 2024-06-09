namespace AppIdentity.Domain;

public class InstanceGroupPermission
{
    public int Id { get; set; }
    public int InstancePermissionId { get; set; }
    public AppGroup Group { get; set; }
    public int GroupId { get; set; }
}