namespace AppIdentity.Domain;

public class AppCredential
{
    public string AppId { get; set; }
    public string SecretKey { get; set; }
    public string AppName { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime LastUpdatedDate { get; set; }
    public bool IsActive { get; set; }
    public string OwnerEntity { get; set; }
}
