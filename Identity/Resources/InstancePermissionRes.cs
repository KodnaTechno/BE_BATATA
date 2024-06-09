namespace AppIdentity.Resources;

public record InstancePermissionRes
{
    public int InstanceId { get; set; }
    public int? RefId { get; set; }
    public List<KeyValuePair<string, object>> ExtraInfo { get; set; }
}