namespace SimpleShop.Api.Models;

/// <summary>
/// Data transfer object representing a cart item returned to the client.
/// </summary>
public class CartItemResponseDto
{
    /// <summary>
    /// Gets or sets the cart item identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the related product identifier.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the product name.
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unit price of the product.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets the selected quantity.
    /// </summary>
    public int Quantity { get; set; }
}