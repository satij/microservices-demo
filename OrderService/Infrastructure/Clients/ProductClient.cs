using System.Net.Http.Json;

namespace OrderService.Infrastructure.Clients;

public class ProductClient : IProductClient
{
    private readonly HttpClient _httpClient;

    public ProductClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ProductDto?> GetProductAsync(int productId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ProductDto>(
                $"api/products/{productId}");
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }
}