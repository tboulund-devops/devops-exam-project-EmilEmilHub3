using System.ComponentModel.DataAnnotations;

namespace SimpleShop.Api.Models;

/// <summary>
/// Data transfer object used to create a new product.
/// </summary>
public class CreateProductDto
{
    /// <summary>
    /// Gets or sets the product name.
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product price.
    /// </summary>
    [Required]
    public decimal? Price { get; set; }
}