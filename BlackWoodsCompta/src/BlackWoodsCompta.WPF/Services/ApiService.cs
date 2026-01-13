using RestSharp;
using Newtonsoft.Json;
using Serilog;

namespace BlackWoodsCompta.WPF.Services;

public class ApiService : IApiService
{
    private RestClient _client;
    public string BaseUrl { get; set; } = "http://localhost:5000";
    public string? Token { get; set; }

    public ApiService()
    {
        _client = new RestClient(BaseUrl);
    }

    public void UpdateBaseUrl(string baseUrl)
    {
        BaseUrl = baseUrl;
        _client = new RestClient(BaseUrl);
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        try
        {
            var request = new RestRequest(endpoint, Method.Get);
            if (!string.IsNullOrEmpty(Token))
            {
                request.AddHeader("Authorization", $"Bearer {Token}");
            }

            var response = await _client.ExecuteAsync(request);
            
            if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
            {
                return JsonConvert.DeserializeObject<T>(response.Content);
            }
            
            Log.Error($"API GET Error: {endpoint} - {response.StatusCode} - {response.ErrorMessage}");
            return default;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Exception in GetAsync: {endpoint}");
            return default;
        }
    }

    public async Task<T?> PostAsync<T>(string endpoint, object data)
    {
        try
        {
            var request = new RestRequest(endpoint, Method.Post);
            if (!string.IsNullOrEmpty(Token))
            {
                request.AddHeader("Authorization", $"Bearer {Token}");
            }
            request.AddJsonBody(data);

            var response = await _client.ExecuteAsync(request);
            
            if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
            {
                return JsonConvert.DeserializeObject<T>(response.Content);
            }
            
            Log.Error($"API POST Error: {endpoint} - {response.StatusCode} - {response.ErrorMessage}");
            return default;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Exception in PostAsync: {endpoint}");
            return default;
        }
    }

    public async Task<T?> PutAsync<T>(string endpoint, object data)
    {
        try
        {
            var request = new RestRequest(endpoint, Method.Put);
            if (!string.IsNullOrEmpty(Token))
            {
                request.AddHeader("Authorization", $"Bearer {Token}");
            }
            request.AddJsonBody(data);

            var response = await _client.ExecuteAsync(request);
            
            if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
            {
                return JsonConvert.DeserializeObject<T>(response.Content);
            }
            
            Log.Error($"API PUT Error: {endpoint} - {response.StatusCode} - {response.ErrorMessage}");
            return default;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Exception in PutAsync: {endpoint}");
            return default;
        }
    }

    public async Task<bool> DeleteAsync(string endpoint)
    {
        try
        {
            var request = new RestRequest(endpoint, Method.Delete);
            if (!string.IsNullOrEmpty(Token))
            {
                request.AddHeader("Authorization", $"Bearer {Token}");
            }

            var response = await _client.ExecuteAsync(request);
            return response.IsSuccessful;
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Exception in DeleteAsync: {endpoint}");
            return false;
        }
    }
}
