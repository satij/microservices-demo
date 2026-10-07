using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Events;

namespace OrderService.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IEventPublisher _eventPublisher;

    public OrderService(IOrderRepository orderRepository, IEventPublisher eventPublisher)
    {
        _orderRepository = orderRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task<Order?> GetOrderAsync(int id)
    {
        return await _orderRepository.GetByIdAsync(id);
    }

    public async Task<Order> CreateOrderAsync(Order order)
    {
        var createdOrder = await _orderRepository.AddAsync(order);

        var orderCreatedEvent = new OrderCreatedEvent
        {
            OrderId = createdOrder.Id,
            CustomerId = createdOrder.CustomerId,
            Total = createdOrder.Total,
            Status = createdOrder.Status
        };

        await _eventPublisher.PublishAsync(
            "OrderCreated",
            orderCreatedEvent);

        return createdOrder;
    }
}