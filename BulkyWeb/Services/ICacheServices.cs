namespace BulkyWeb.Services
{
    public interface ICacheServices
    {
        Task<T?> GetAsync<T>(string Key, CancellationToken cancellationToken=default);
        Task SetAsync<T>(string Key,T value, CancellationToken cancellationToken=default);
        Task RemoveAsync<T>(string key, CancellationToken cancellationToken=default);   
    }
}
