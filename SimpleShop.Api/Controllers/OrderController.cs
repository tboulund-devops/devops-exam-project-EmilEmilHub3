using Microsoft.AspNetCore.Mvc;
using SimpleShop.Api.Models;
using SimpleShop.Api.Services;

namespace SimpleShop.Api.Controllers;

/// <summary>
/// Provides endpoints for creating customer orders.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrdersController"/> class.
    /// </summary>
    /// <param name="service">Service responsible for order business logic.</param>
    public OrdersController(OrderService service)
    {
        _service = service;
    }

    /// <summary>
    /// Creates a new order from the supplied request data.
    /// </summary>
    /// <param name="dto">The order creation data.</param>
    /// <returns>
    /// A <c>201 Created</c> response containing the created order when successful,
    /// or a <c>400 Bad Request</c> response if the request data is invalid.
    /// </returns>
    [HttpPost]
    public async Task<ActionResult<OrderResponseDto>> Create([FromBody] CreateOrderDto dto)
    {
        try
        {
            var created = await _service.CreateOrderAsync(dto);
            return Created($"/api/orders/{created.Id}", created);
        }
        catch (ArgumentException ex)
        {
            // Return a validation error if the order request is invalid.
            return BadRequest(new { error = ex.Message });
        }
    }
}