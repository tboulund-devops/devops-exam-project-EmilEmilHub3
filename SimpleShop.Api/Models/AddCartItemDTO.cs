using System.ComponentModel.DataAnnotations;

namespace SimpleShop.Api.Models;

public class AddCartItemDto
{
    [Required]
    public int? UserId { get; set; }

    [Required]
    public int? ProductId { get; set; }

    [Required]
    public int? Quantity { get; set; }
}