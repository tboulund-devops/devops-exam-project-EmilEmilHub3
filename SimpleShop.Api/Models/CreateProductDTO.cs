namespace SimpleShop.Api.Models;

public class CreateProductDTO
{
    public string Name { get; set; } = string.Empty;
    public decimal? Price { get; set; }
}