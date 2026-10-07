using OrderService.Domain.Entities;

public interface IOrderService
{
    Task<Order?> GetOrderAsync(int id);
    Task<Order> CreateOrderAsync(Order order);
}