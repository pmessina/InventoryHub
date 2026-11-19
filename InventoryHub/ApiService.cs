using ClientApp.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ClientApp
{
    public class ApiService
    {
        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        private readonly HttpClient _httpClient;

        public async Task<Product[]> GetApiResponseAsync()
        {
            var response = await _httpClient.GetAsync("api/products");
            response.EnsureSuccessStatusCode();
            var products = await response.Content.ReadFromJsonAsync<Product[]>() ?? Array.Empty<Product>();
            return products;
        }
    }
}
