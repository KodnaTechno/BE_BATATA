namespace AppIdentity.Domain;

public class AppGroup
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsActivated { get; set; }
    public ICollection<AppGroupUser> GroupUsers { get; set; }
    public DateTime CreatedAt { get; set; }

    public AppGroup()
    {
        IsActivated = true;
        CreatedAt = DateTime.UtcNow;
    }
}