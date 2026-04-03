
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace BulkyWeb.Services
{
    public class CacheServices(IDistributedCache distributedCache) : ICacheServices
    {
        private readonly IDistributedCache _distributedCache = distributedCache;

        public async Task<T?> GetAsync<T>(string Key, CancellationToken cancellationToken = default)  
        {
            var cachedValue=await _distributedCache.GetStringAsync(Key, cancellationToken);
            return cachedValue is null ?
                default : JsonSerializer.Deserialize<T>(cachedValue);
        }

        

        public async Task SetAsync<T>(string Key, T value, CancellationToken cancellationToken = default) 
        {
            await _distributedCache.SetStringAsync(Key, JsonSerializer.Serialize(value), cancellationToken);
        }
        public async Task RemoveAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            await _distributedCache.RemoveAsync(key, cancellationToken);
        }
    }
}
