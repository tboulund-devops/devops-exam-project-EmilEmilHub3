using System.ComponentModel.DataAnnotations;

namespace SimpleShop.Api.Models;

/// <summary>
/// Data transfer object used to create a new order.
/// </summary>
public class CreateOrderDto
{
    /// <summary>
    /// Gets or sets the identifier of the user placing the order.
    /// </summary>
    [Required]
    public int? UserId { get; set; }
}