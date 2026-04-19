namespace SimpleShop.Api.Models;

/// <summary>
/// Data transfer object representing a complete shopping cart.
/// </summary>
public class CartResponseDto
{
    /// <summary>
    /// Gets or sets the identifier of the cart owner.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the cart items.
    /// </summary>
    public List<CartItemResponseDto> Items { get; set; } = new();

    /// <summary>
    /// Gets or sets the total calculated cart price.
    /// </summary>
    public decimal TotalPrice { get; set; }
}