namespace AppIdentity;

using System.Collections.Generic;

public static class TokenRevocationList
{
    private static readonly List<string> RevokedTokens = new();

    public static void RevokeToken(string token)
    {
        lock (RevokedTokens)
        {
            RevokedTokens.Add(token);
        }
    }

    public static bool IsTokenRevoked(string token)
    {
        lock (RevokedTokens)
        {
            return RevokedTokens.Contains(token);
        }
    }
}