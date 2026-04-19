using System.ComponentModel.DataAnnotations;

namespace SimpleShop.Api.Models;

/// <summary>
/// Data transfer object used to add a product to a user's shopping cart.
/// </summary>
public class AddCartItemDto
{
    /// <summary>
    /// Gets or sets the identifier of the user who owns the cart.
    /// </summary>
    [Required]
    public int? UserId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the product to add.
    /// </summary>
    [Required]
    public int? ProductId { get; set; }

    /// <summary>
    /// Gets or sets the quantity of the selected product.
    /// </summary>
    [Required]
    public int? Quantity { get; set; }
}