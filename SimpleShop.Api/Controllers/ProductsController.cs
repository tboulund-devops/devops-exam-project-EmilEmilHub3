using Microsoft.AspNetCore.Mvc;
using SimpleShop.Api.Models;
using SimpleShop.Api.Services;

namespace SimpleShop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _service;

    public ProductsController(ProductService service)
    {
        _service = service;
    }

    // Week 6 Feature 1: GET /api/products
    [HttpGet]
    public async Task<ActionResult<List<Product>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    // Week 6 Feature 2: POST /api/products
    [HttpPost]
    public async Task<ActionResult<Product>> Create([FromBody] CreateProductDto dto)
    {
        try
        {
            var created = await _service.CreateAsync(dto);
            return Created($"/api/products/{created.Id}", created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}