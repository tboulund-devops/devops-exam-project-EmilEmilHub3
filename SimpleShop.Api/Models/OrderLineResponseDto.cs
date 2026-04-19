namespace SimpleShop.Api.Models;

/// <summary>
/// Data transfer object representing a single order line returned to the client.
/// </summary>
public class OrderLineResponseDto
{
    /// <summary>
    /// Gets or sets the identifier of the product.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the product name.
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ordered quantity.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price used for the order line.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the calculated total for the order line.
    /// </summary>
    public decimal LineTotal { get; set; }
}