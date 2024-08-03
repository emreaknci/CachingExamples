using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace DistributedCaching.Services
{
    public class ProductCacheService : RedisCacheService
    {
        public ProductCacheService(IDistributedCache distributedCache, IConnectionMultiplexer connectionMultiplexer) : base(distributedCache, connectionMultiplexer)
        {
        }

        public async Task RemoveProductFilterCacheAsync()
        {
            var productFilterKeys = GetAllCacheKeys().Where(k => k.StartsWith("ProductFilter_"));

            foreach (var key in productFilterKeys)          
                await RemoveCacheAsync(key);     
        }
    }
}

