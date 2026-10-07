using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using OrderService.Application.Interfaces;

namespace OrderService.Infrastructure.Messaging;

public class RabbitMqEventPublisher : IEventPublisher
{
    private readonly IConfiguration _configuration;

    public RabbitMqEventPublisher(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task PublishAsync<T>(
        string eventName,
        T message)
    {
        var rabbitMq = _configuration.GetSection("RabbitMQ");

        var factory = new ConnectionFactory
        {
            HostName = rabbitMq["Host"],
            Port = int.Parse(rabbitMq["Port"] ?? "5672"),
            UserName = rabbitMq["Username"],
            Password = rabbitMq["Password"]
        };

        await using var connection =
            await factory.CreateConnectionAsync();

        await using var channel =
            await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(
            exchange: "orders",
            type: ExchangeType.Fanout,
            durable: true);

        var body = Encoding.UTF8.GetBytes(
            JsonSerializer.Serialize(message));

        await channel.BasicPublishAsync(
            exchange: "orders",
            routingKey: string.Empty,
            body: body);
    }
}