namespace SimpleShop.Api.Models;

/// <summary>
/// Represents a single product line within an order.
/// Stores the ordered product, quantity,
/// and the unit price at the time of purchase.
/// </summary>
public class OrderLine
{
    /// <summary>
    /// Gets or sets the order line identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the related order.
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the related product.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the quantity of the product in the order.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price of the product at the time the order was created.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the related order entity.
    /// </summary>
    public Order? Order { get; set; }

    /// <summary>
    /// Gets or sets the related product entity.
    /// </summary>
    public Product? Product { get; set; }
}