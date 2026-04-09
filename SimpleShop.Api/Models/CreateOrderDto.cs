using System.ComponentModel.DataAnnotations;

namespace SimpleShop.Api.Models;

public class CreateOrderDto
{
    [Required]
    public int? UserId { get; set; }
}