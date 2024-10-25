using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;

namespace Infrastructure.Caching
{
    public class EntityCacheService : IEntityCacheService
    {
        private readonly ICacheProvider _cacheProvider;

        public EntityCacheService(ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory) where T : class
        {
            var cachedValue = await _cacheProvider.GetAsync(key);
            if (cachedValue != null)
                return JsonSerializer.Deserialize<T>(cachedValue);

            var value = await factory();
            if (value != null)
                await CacheValueAsync(key, value);

            return value;
        }

        public async Task<List<T>> GetOrSetListAsync<T>(string key, Func<Task<List<T>>> factory) where T : class
        {
            var cachedValue = await _cacheProvider.GetAsync(key);
            if (cachedValue != null)
                return JsonSerializer.Deserialize<List<T>>(cachedValue);

            var value = await factory();
            if (value != null && value.Count > 0)
                await CacheValueAsync(key, value);

            return value;
        }

        private async Task CacheValueAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class
        {
            var serializedValue = JsonSerializer.SerializeToUtf8Bytes(value);
            await _cacheProvider.SetAsync(key, serializedValue, expiry);
        }

        public async Task RemoveAsync(string key)
        {
            await _cacheProvider.RemoveAsync(key);
        }

        public async Task RemoveByPrefixAsync(string prefix)
        {
            await _cacheProvider.RemoveByPrefixAsync(prefix);
        }
    }
}
