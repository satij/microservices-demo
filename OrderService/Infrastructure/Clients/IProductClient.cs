using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Clients;

public interface IProductClient
{
    Task<ProductDto?> GetProductAsync(int productId);
}