namespace SimpleShop.Api.Models;

/// <summary>
/// Represents a customer order.
/// </summary>
public class Order
{
    /// <summary>
    /// Gets or sets the order identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who placed the order.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the related user entity.
    /// </summary>
    public User? User { get; set; }

    /// <summary>
    /// Gets or sets the products included in the order.
    /// </summary>
    public List<OrderLine> OrderLines { get; set; } = new();
}