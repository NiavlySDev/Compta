namespace BlackWoodsCompta.WPF.Services;

public interface IApiService
{
    string BaseUrl { get; set; }
    string? Token { get; set; }
    Task<T?> GetAsync<T>(string endpoint);
    Task<T?> PostAsync<T>(string endpoint, object data);
    Task<T?> PutAsync<T>(string endpoint, object data);
    Task<bool> DeleteAsync(string endpoint);
}
