using System.Text;
using System.Text.Json;
using InventoryService.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace InventoryService.Messaging;

public class OrderCreatedConsumer : BackgroundService
{
    private readonly IConfiguration _configuration;

    public OrderCreatedConsumer(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var rabbitMq = _configuration
            .GetSection("RabbitMQ");

        var factory = new ConnectionFactory
        {
            HostName = rabbitMq["Host"],
            Port = int.Parse(rabbitMq["Port"] ?? "5672"),
            UserName = rabbitMq["Username"],
            Password = rabbitMq["Password"]
        };

        var connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(
            exchange: "orders",
            type: ExchangeType.Fanout,
            durable: true);

        await channel.QueueDeclareAsync(
            queue: "order-created",
            durable: true,
            exclusive: false,
            autoDelete: false);

        await channel.QueueBindAsync(
            queue: "order-created",
            exchange: "orders",
            routingKey: string.Empty);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, args) =>
        {
            var json = Encoding.UTF8.GetString(args.Body.ToArray());

            var orderCreated =
                JsonSerializer.Deserialize<OrderCreatedEvent>(json);

            Console.WriteLine(
                $"Order received: OrderId={orderCreated?.OrderId}, " +
                $"CustomerId={orderCreated?.CustomerId}, " +
                $"Total={orderCreated?.Total}, " +
                $"Status={orderCreated?.Status}");

            await channel.BasicAckAsync(
                args.DeliveryTag,
                multiple: false);
        };

        await channel.BasicConsumeAsync(
            queue: "order-created",
            autoAck: false,
            consumer: consumer);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}