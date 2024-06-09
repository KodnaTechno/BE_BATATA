namespace AppIdentity.Domain;

public class AppGroupUser
{
    public int Id { get; set; }

    public string UserId { get; set; }
    public AppUser User { get; set; }
    public int GroupId { get; set; }
    public AppGroup Group { get; set; }
    public DateTime CreatedAt { get; set; }

    public AppGroupUser()
    {
        CreatedAt = DateTime.UtcNow;
    }
}