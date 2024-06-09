using AppIdentity.Domain;

namespace AppIdentity.Resources;

public class AuthenticateResult
{
    public AppUser AppUser { get; set; }
    public bool Succeeded { get; set; }
    public string Token { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsLocked { get; set; }
}