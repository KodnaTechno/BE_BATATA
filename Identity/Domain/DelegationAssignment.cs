namespace AppIdentity.Domain;

public class DelegationAssignment
{
    public int Id { get; set; }
    public string DelegateFromId { get; set; }
    public AppUser DelegateFrom { get; set; }
    public string DelegateToId { get; set; }
    public AppUser DelegateTo { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public DelegationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public enum DelegationStatus
{
    Expired, Active, Deactivated
}
