using Microsoft.AspNetCore.Mvc;
using SimpleShop.Api.FeatureFlags;
using SimpleShop.Api.Models;
using SimpleShop.Api.Services;

namespace SimpleShop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _service;
    private readonly FeatureDecisions? _featureDecisions;

    public ProductsController(ProductService service, FeatureDecisions? featureDecisions = null)
    {
        _service = service;
        _featureDecisions = featureDecisions;
    }


    [HttpGet]
    public async Task<ActionResult<List<Product>>> GetAll([FromQuery] string? search)
    {
        if (!string.IsNullOrWhiteSpace(search) &&
            _featureDecisions is not null &&
            !_featureDecisions.CanSearchProducts())
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable,
                new { error = "Product search is currently disabled by feature toggle." });
        }

        return Ok(await _service.GetAllAsync(search));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        try
        {
            var product = await _service.GetByIdAsync(id);
            return product is null ? NotFound() : Ok(product);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

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

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Product>> Update(int id, [FromBody] CreateProductDto dto)
    {
        try
        {
            var updated = await _service.UpdateAsync(id, dto);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (_featureDecisions is not null && !_featureDecisions.CanDeleteProducts())
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable,
                new { error = "Product delete is currently disabled by feature toggle." });
        }

        try
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}