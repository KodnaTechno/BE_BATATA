namespace AppIdentity.Domain;

public class AppAccessibility
{
    public int Id { get; set; }
    public string ModuleKey { get; set; }
    public string ModuleName { get; set; }
    public string Category { get; set; }
    public string CategoryName { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<AppAccessibilityGroup> AccessibilityGroups  { get; set; }

    public AppAccessibility()
    {
        CreatedAt = DateTime.UtcNow;
    }
}