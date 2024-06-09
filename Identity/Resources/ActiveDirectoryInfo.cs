namespace AppIdentity.Resources;

public struct ActiveDirectoryInfo
{
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Address { get; set; }
    public string Domain { get; set; }
    public string DistinguishedName { get; set; }
}