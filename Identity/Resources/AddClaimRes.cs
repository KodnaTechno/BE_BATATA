namespace AppIdentity.Resources;

public record AddClaimRes
{
    public string ClaimType { get; set; }
    public string ClaimValue { get; set; }
    public string ClaimValueRef { get; set; }
}