using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Infrastructure.Clients;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IProductClient _productClient;


    public OrdersController(IOrderService orderService, IProductClient productClient)
    {
        _orderService = orderService;
        _productClient = productClient;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        var order = await _orderService.GetOrderAsync(id);

        if (order == null)
            return NotFound();

        return Ok(order);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(Order order)
    {
        var createdOrder = await _orderService.CreateOrderAsync(order);

        return CreatedAtAction(
            nameof(GetOrder),
            new { id = createdOrder.Id },
            createdOrder);
    }

    [HttpGet("product/{productId:int}")]
    public async Task<IActionResult> GetProduct(int productId)
    {
        var product = await _productClient.GetProductAsync(productId);

        if (product == null)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                "Product service is currently unavailable.");
        }

        return Ok(product);
    }
}