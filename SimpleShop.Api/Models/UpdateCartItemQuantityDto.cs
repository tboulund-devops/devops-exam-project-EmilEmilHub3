using System.ComponentModel.DataAnnotations;

namespace SimpleShop.Api.Models;

public class UpdateCartItemQuantityDto
{
    [Required]
    public int? Quantity { get; set; }
}