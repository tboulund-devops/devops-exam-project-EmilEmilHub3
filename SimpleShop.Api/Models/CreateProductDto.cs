using System.ComponentModel.DataAnnotations;

namespace SimpleShop.Api.Models;

public class CreateProductDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public decimal? Price { get; set; }
}
