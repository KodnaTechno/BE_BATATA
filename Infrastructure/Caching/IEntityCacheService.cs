﻿namespace Infrastructure.Caching
{
    public interface IEntityCacheService
    {
        Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null) where T : class;
        Task<List<T>> GetOrSetListAsync<T>(string key, Func<Task<List<T>>> factory, TimeSpan? expiry = null) where T : class;
        Task RemoveAsync(string key);
        Task RemoveByPrefixAsync(string prefix);
    }
}
