namespace Infrastructure.Caching
{
    public interface ICacheProvider
    {
        Task<byte[]> GetAsync(string key);
        Task SetAsync(string key, byte[] value, TimeSpan? expiry = null);
        Task RemoveAsync(string key);
        Task RemoveByPrefixAsync(string prefix);
    }
}
