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
    public async Task<ActionResult<CartItemResponseDTO>> AddItem([FromBody] AddCartItemDTO dto)
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
    public async Task<ActionResult<CartResponseDTO>> GetCart([FromQuery] int userId)
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
}