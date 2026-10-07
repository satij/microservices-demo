using OrderService.Domain.Entities;

namespace OrderService.Application.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id);

    Task<Order> AddAsync(Order order);

    Task SaveChangesAsync();
}