using AppIdentity.Resources;

namespace AppIdentity.IServices;

public interface IActiveDirectoryInfoService
{
    public ActiveDirectoryInfo GetActiveDirectoryInfo();
    public string MobileMapping();
    public string ExtMapping();
    public string EmailMapping();
    public string DisplayNameMapping();
    public List<string> GetProviderExtraInformation();
    public static readonly string Username = "sAMAccountName";
    // public static string DisplayName = "displayName";

}