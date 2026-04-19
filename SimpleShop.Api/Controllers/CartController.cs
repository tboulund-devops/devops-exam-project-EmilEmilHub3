using Microsoft.AspNetCore.Mvc;
using SimpleShop.Api.Models;
using SimpleShop.Api.Services;

namespace SimpleShop.Api.Controllers;

/// <summary>
/// Provides endpoints for managing a user's shopping cart.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly CartService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="CartController"/> class.
    /// </summary>
    /// <param name="service">Service responsible for cart business logic.</param>
    public CartController(CartService service)
    {
        _service = service;
    }

    /// <summary>
    /// Adds an item to the cart.
    /// </summary>
    /// <param name="dto">The cart item data to add.</param>
    /// <returns>
    /// A <c>201 Created</c> response with the created cart item when successful,
    /// or a <c>400 Bad Request</c> response if the input is invalid.
    /// </returns>
    [HttpPost("items")]
    public async Task<ActionResult<CartItemResponseDto>> AddItem([FromBody] AddCartItemDto dto)
    {
        try
        {
            var created = await _service.AddItemAsync(dto);
            return Created($"/api/cart/items/{created.Id}", created);
        }
        catch (ArgumentException ex)
        {
            // Return a validation error if the request contains invalid cart data.
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Gets the current cart for a specific user.
    /// </summary>
    /// <param name="userId">The identifier of the user whose cart should be returned.</param>
    /// <returns>
    /// A <c>200 OK</c> response containing the cart,
    /// or a <c>400 Bad Request</c> response if the input is invalid.
    /// </returns>
    [HttpGet]
    public async Task<ActionResult<CartResponseDto>> GetCart([FromQuery] int userId)
    {
        try
        {
            return Ok(await _service.GetCartAsync(userId));
        }
        catch (ArgumentException ex)
        {
            // Return a validation error if the user id is invalid.
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Updates the quantity of an existing cart item.
    /// </summary>
    /// <param name="id">The identifier of the cart item to update.</param>
    /// <param name="dto">The updated quantity information.</param>
    /// <returns>
    /// A <c>200 OK</c> response with the updated cart item when successful,
    /// a <c>404 Not Found</c> response if the cart item does not exist,
    /// or a <c>400 Bad Request</c> response if the input is invalid.
    /// </returns>
    [HttpPut("items/{id}")]
    public async Task<ActionResult<CartItemResponseDto>> UpdateItemQuantity(int id, [FromBody] UpdateCartItemQuantityDto dto)
    {
        try
        {
            var updated = await _service.UpdateItemQuantityAsync(id, dto);
            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            // Return not found if the requested cart item does not exist.
            return NotFound(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            // Return a validation error if the request data is invalid.
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Removes an item from the cart.
    /// </summary>
    /// <param name="id">The identifier of the cart item to remove.</param>
    /// <returns>
    /// A <c>204 No Content</c> response when the item is removed successfully,
    /// a <c>404 Not Found</c> response if the item does not exist,
    /// or a <c>400 Bad Request</c> response if the input is invalid.
    /// </returns>
    [HttpDelete("items/{id}")]
    public async Task<IActionResult> RemoveItem(int id)
    {
        try
        {
            await _service.RemoveItemAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            // Return not found if the requested cart item does not exist.
            return NotFound(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            // Return a validation error if the request data is invalid.
            return BadRequest(new { error = ex.Message });
        }
    }
}