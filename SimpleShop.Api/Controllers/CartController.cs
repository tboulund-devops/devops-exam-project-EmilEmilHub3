using Microsoft.AspNetCore.Mvc;
using SimpleShop.Api.Models;
using SimpleShop.Api.Services;

namespace SimpleShop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly CartService _service;

    public CartController(CartService service)
    {
        _service = service;
    }

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
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<CartResponseDto>> GetCart([FromQuery] int userId)
    {
        try
        {
            return Ok(await _service.GetCartAsync(userId));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

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
            return NotFound(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

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
            return NotFound(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}