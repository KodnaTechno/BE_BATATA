using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

namespace Infrastructure.Caching.Providers
{
    public class MemoryCacheProvider : ICacheProvider
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ConcurrentDictionary<string, HashSet<string>> _prefixIndex;

        public MemoryCacheProvider(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _prefixIndex = new ConcurrentDictionary<string, HashSet<string>>();
        }

        public Task<byte[]> GetAsync(string key)
        {
            return Task.FromResult(_memoryCache.TryGetValue(key, out byte[] value) ? value : null);
        }

        public Task SetAsync(string key, byte[] value, TimeSpan? expiry = null)
        {
            var options = new MemoryCacheEntryOptions();
            if (expiry.HasValue)
            {
                options.AbsoluteExpirationRelativeToNow = expiry.Value;
            }

            _memoryCache.Set(key, value, options);

            var prefixes = GetPrefixes(key);
            foreach (var prefix in prefixes)
            {
                _prefixIndex.AddOrUpdate(prefix,
                    _ => new HashSet<string> { key },
                    (_, hashSet) =>
                    {
                        hashSet.Add(key);
                        return hashSet;
                    });
            }

            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            _memoryCache.Remove(key);

            var prefixes = GetPrefixes(key);
            foreach (var prefix in prefixes)
            {
                if (_prefixIndex.TryGetValue(prefix, out var hashSet))
                {
                    hashSet.Remove(key);
                    if (hashSet.Count == 0)
                    {
                        _prefixIndex.TryRemove(prefix, out _);
                    }
                }
            }

            return Task.CompletedTask;
        }

        public Task RemoveByPrefixAsync(string prefix)
        {
            if (_prefixIndex.TryGetValue(prefix, out var hashSet))
            {
                foreach (var key in hashSet)
                {
                    _memoryCache.Remove(key);
                }
                _prefixIndex.TryRemove(prefix, out _);
            }
            return Task.CompletedTask;
        }

        private IEnumerable<string> GetPrefixes(string key)
        {
            for (int i = 1; i <= key.Length; i++)
            {
                yield return key.Substring(0, i);
            }
        }
    }
}
