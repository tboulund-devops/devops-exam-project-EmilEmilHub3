using System.ComponentModel.DataAnnotations;

namespace SimpleShop.Api.Models;

/// <summary>
/// Data transfer object used to update the quantity of an existing cart item.
/// </summary>
public class UpdateCartItemQuantityDto
{
    /// <summary>
    /// Gets or sets the new quantity value.
    /// </summary>
    [Required]
    public int? Quantity { get; set; }
}