using Infrastructure.Caching.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Caching
{
    public class CacheProviderFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public CacheProviderFactory(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        public ICacheProvider CreateCacheProvider()
        {
            var cacheType = _configuration.GetValue<string>("CacheSettings:Provider");

            return cacheType?.ToLower() switch
            {
                "redis" => _serviceProvider.GetRequiredService<RedisCacheProvider>(),
                "memory" => _serviceProvider.GetRequiredService<MemoryCacheProvider>(),
                _ => _serviceProvider.GetRequiredService<MemoryCacheProvider>()
            };
        }
    }
}
