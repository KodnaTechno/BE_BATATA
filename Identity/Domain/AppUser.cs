using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;

namespace AppIdentity.Domain;

public class AppUser : IdentityUser
{
    public string DisplayName { get; set; }
    public int Stars { get; set; }
    public string Mobile { get; set; }
    public string Ext { get; set; }
    
    public bool IsExternal { get; set; }
    public List<KeyValuePair<string, string>> ExtraInfo { get; set; }
    
    public List<KeyValuePair<string, string>> ProviderExtraInfo { get; set; }
    public string Image { get; set; }
    public ICollection<AppGroupUser> GroupUsers { get; set; }
    public DateTime CreatedAt { get; set; }
}