using Microsoft.AspNetCore.Mvc;
using SimpleShop.Api.FeatureFlags;
using SimpleShop.Api.Models;
using SimpleShop.Api.Services;

namespace SimpleShop.Api.Controllers;

/// <summary>
/// Provides endpoints for managing products.
/// Supports feature toggle checks for product search and product deletion.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _service;
    private readonly FeatureDecisions? _featureDecisions;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductsController"/> class.
    /// </summary>
    /// <param name="service">Service responsible for product business logic.</param>
    /// <param name="featureDecisions">
    /// Optional feature decision service used to evaluate feature toggles.
    /// </param>
    public ProductsController(ProductService service, FeatureDecisions? featureDecisions = null)
    {
        _service = service;
        _featureDecisions = featureDecisions;
    }

    /// <summary>
    /// Gets all products, optionally filtered by a search term.
    /// </summary>
    /// <param name="search">Optional search term used to filter products by name.</param>
    /// <returns>
    /// A <c>200 OK</c> response containing the matching products,
    /// or a <c>503 Service Unavailable</c> response if product search is disabled by feature toggle.
    /// </returns>
    [HttpGet]
    public async Task<ActionResult<List<Product>>> GetAll([FromQuery] string? search)
    {
        // Block search requests when the search feature is disabled.
        if (!string.IsNullOrWhiteSpace(search) &&
            _featureDecisions is not null &&
            !await _featureDecisions.CanSearchProducts())
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable,
                new { error = "Product search is currently disabled by feature toggle." });
        }

        return Ok(await _service.GetAllAsync(search));
    }

    /// <summary>
    /// Gets a single product by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the product.</param>
    /// <returns>
    /// A <c>200 OK</c> response with the product when found,
    /// a <c>404 Not Found</c> response when the product does not exist,
    /// or a <c>400 Bad Request</c> response if the input is invalid.
    /// </returns>
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
            // Return a validation error if the supplied id is invalid.
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="dto">The product data to create.</param>
    /// <returns>
    /// A <c>201 Created</c> response with the created product when successful,
    /// or a <c>400 Bad Request</c> response if the request data is invalid.
    /// </returns>
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
            // Return a validation error if the submitted product data is invalid.
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="id">The identifier of the product to update.</param>
    /// <param name="dto">The updated product data.</param>
    /// <returns>
    /// A <c>200 OK</c> response with the updated product when successful,
    /// a <c>404 Not Found</c> response if the product does not exist,
    /// or a <c>400 Bad Request</c> response if the request data is invalid.
    /// </returns>
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
            // Return a validation error if the supplied update data is invalid.
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Deletes an existing product.
    /// </summary>
    /// <param name="id">The identifier of the product to delete.</param>
    /// <returns>
    /// A <c>204 No Content</c> response when deletion succeeds,
    /// a <c>404 Not Found</c> response when the product does not exist,
    /// a <c>400 Bad Request</c> response if the input is invalid,
    /// or a <c>503 Service Unavailable</c> response if deletion is disabled by feature toggle.
    /// </returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        // Block delete requests when the delete feature is disabled.
        if (_featureDecisions is not null &&
            !await _featureDecisions.CanDeleteProducts())
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
            // Return a validation error if the supplied id is invalid.
            return BadRequest(new { error = ex.Message });
        }
    }
}