using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Threading.Tasks;
namespace ContactList.Web.Common.Services
{
    public class RedisService : IRedisService
    {
        private readonly IDistributedCache _redisCache;

        // Constructor to inject IDistributedCache
        public RedisService(IDistributedCache redisCache)
        {
            _redisCache = redisCache;
        }

        // Implement the method to store any object in Redis
        public async Task StoreDataAsync<T>(string key, T data)
        {
            // Serialize the data into a JSON string
            var jsonData = JsonConvert.SerializeObject(data);

            // Define cache entry options with 30-minute sliding expiration
            var options = new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(30)
            };

            // Store the serialized data in Redis with expiration
            await _redisCache.SetStringAsync(key, jsonData, options);
        }

        // Implement the method to retrieve any object from Redis
        public async Task<T> GetDataAsync<T>(string key)
        {
            // Retrieve the serialized data from Redis
            var jsonData = await _redisCache.GetStringAsync(key);

            if (string.IsNullOrEmpty(jsonData))
            {
                return default; // Return the default value if not found
            }

            // Deserialize the JSON string into the specified object type
            var data = JsonConvert.DeserializeObject<T>(jsonData);
            return data;
        }
    }
}