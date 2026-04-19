namespace SimpleShop.Api.Models;

/// <summary>
/// Represents an item stored in a user's shopping cart.
/// </summary>
public class CartItem
{
    /// <summary>
    /// Gets or sets the cart item identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the owning user identifier.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the related product identifier.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the quantity of the selected product.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the related user entity.
    /// </summary>
    public User? User { get; set; }

    /// <summary>
    /// Gets or sets the related product entity.
    /// </summary>
    public Product? Product { get; set; }
}