namespace SimpleShop.Api.Models;

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public decimal? Price { get; set; }
}