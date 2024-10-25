namespace Infrastructure.Caching
{
    public static class CacheKeys
    {
        public static string Workspace(Guid workspaceId) => $"Workspace:{workspaceId}";
    }
}
