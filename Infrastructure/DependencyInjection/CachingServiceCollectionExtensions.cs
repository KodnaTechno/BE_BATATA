using Infrastructure.Caching;
using Infrastructure.Caching.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Infrastructure.DependencyInjection
{
    public static class CachingServiceCollectionExtensions
    {
        public static IServiceCollection AddFlexibleCaching(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();

            var cacheType = configuration.GetValue<string>("CacheSettings:Provider")?.ToLower();

            if (cacheType == "redis")
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = configuration.GetConnectionString("RedisConnection");
                    options.InstanceName = configuration.GetValue<string>("CacheSettings:Instance");
                });

                services.AddSingleton<IConnectionMultiplexer>(sp =>
                    ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisConnection")));

                services.AddSingleton<RedisCacheProvider>();
            }

            services.AddSingleton<MemoryCacheProvider>();
            services.AddSingleton<CacheProviderFactory>();
            services.AddSingleton(sp =>
            {
                var factory = sp.GetRequiredService<CacheProviderFactory>();
                return factory.CreateCacheProvider();
            });
            services.AddSingleton<IEntityCacheService, EntityCacheService>();

            if (cacheType != "redis")
            {
                services.AddDistributedMemoryCache();
            }

            return services;
        }

    }
}
