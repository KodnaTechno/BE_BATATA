using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace Infrastructure.Caching.Providers
{
    public class RedisCacheProvider : ICacheProvider
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IConnectionMultiplexer _redisConnection;

        public RedisCacheProvider(IDistributedCache distributedCache, IConnectionMultiplexer redisConnection)
        {
            _distributedCache = distributedCache;
            _redisConnection = redisConnection;
        }

        public async Task<byte[]> GetAsync(string key)
            => await _distributedCache.GetAsync(key);

        public async Task SetAsync(string key, byte[] value, TimeSpan? expiry = null) =>
             await _distributedCache.SetAsync(key, value, expiry.HasValue ? new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiry.Value }
             : new DistributedCacheEntryOptions());

        public async Task RemoveAsync(string key)
            => await _distributedCache.RemoveAsync(key);

        public async Task RemoveByPrefixAsync(string prefix)
        {
            var endpoints = _redisConnection.GetEndPoints();
            var tasks = endpoints.Select(async endpoint =>
            {
                var server = _redisConnection.GetServer(endpoint);
                var keys = server.Keys(pattern: $"{prefix}*").ToArray();

                if (keys.Length > 0)
                {
                    var database = _redisConnection.GetDatabase();
                    await database.KeyDeleteAsync(keys);
                }
            });

            await Task.WhenAll(tasks);
        }
    }
}
