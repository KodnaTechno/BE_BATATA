namespace AppIdentity.Resources;

public record AddUserRes
{ 
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string DisplayName { get; set; }
    public int Stars { get; set; }
    public string Mobile { get; set; }
    public string Ext { get; set; }
    public List<KeyValuePair<string, string>> ExtraInfo { get; set; }
    public string Image { get; set; }

}