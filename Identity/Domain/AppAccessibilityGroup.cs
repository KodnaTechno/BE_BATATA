namespace AppIdentity.Domain;

public class AppAccessibilityGroup
{
    public int Id { get; set; }
    public AppAccessibility AppAccessibility { get; set; }
    public int AppAccessibilityId { get; set; }
    public AppGroup AppGroup { get; set; }
    public int AppGroupId { get; set; }
    public DateTime CreatedAt { get; set; }

    public AppAccessibilityGroup()
    {
        CreatedAt = DateTime.UtcNow;
    }
}