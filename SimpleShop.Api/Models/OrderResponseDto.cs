namespace SimpleShop.Api.Models;

/// <summary>
/// Data transfer object representing an order returned to the client.
/// </summary>
public class OrderResponseDto
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
    /// Gets or sets the order lines included in the order.
    /// </summary>
    public List<OrderLineResponseDto> OrderLines { get; set; } = new();

    /// <summary>
    /// Gets or sets the total price of the entire order.
    /// </summary>
    public decimal TotalPrice { get; set; }
}