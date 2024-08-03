using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text.Json;

namespace DistributedCaching.Services
{
    public class RedisCacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public RedisCacheService(IDistributedCache distributedCache, IConnectionMultiplexer connectionMultiplexer)
        {
            _distributedCache = distributedCache;
            _connectionMultiplexer = connectionMultiplexer;
        }

        public async Task<T?> GetFromCacheAsync<T>(string cacheKey) where T : class
        {
            var cachedData = await _distributedCache.GetStringAsync(cacheKey);
            return cachedData != null ? JsonSerializer.Deserialize<T>(cachedData) : null;
        }

        public async Task SetCacheAsync<T>(string cacheKey, T data, DistributedCacheEntryOptions options) where T : class
        {
            var serializedData = JsonSerializer.Serialize(data);
            await _distributedCache.SetStringAsync(cacheKey, serializedData, options);
        }

        public IEnumerable<string> GetAllCacheKeys()
        {
            var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints().First());
            var keys = server.Keys();
            return keys.Select(key => key.ToString());
        }

        public async Task RemoveCacheAsync(string cacheKey)
            => await _distributedCache.RemoveAsync(cacheKey);

    }
}

