namespace AppIdentity.IServices;

public interface ISettingsService
{
    public string UsernameMapping();
    public bool IsSSOEnabled();
}