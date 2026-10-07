namespace OrderService.Application.Interfaces;

public interface IEventPublisher
{
    Task PublishAsync<T>(string eventName, T message);
}