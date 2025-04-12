namespace ContactList.Web.Common.Services
{
    public interface IRedisService
    {
        // Generic method to store any object in Redis
        Task StoreDataAsync<T>(string key, T data);

        // Generic method to retrieve any object from Redis
        Task<T> GetDataAsync<T>(string key);
    }
}