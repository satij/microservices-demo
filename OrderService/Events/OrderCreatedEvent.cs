namespace OrderService.Events;

public class OrderCreatedEvent
{
    public int OrderId { get; set; }

    public int CustomerId { get; set; }

    public decimal Total { get; set; }

    public string Status { get; set; } = "Created";
}