namespace SimpleShop.Api.Models;

public class CartResponseDto
{
    public int UserId { get; set; }
    public List<CartItemResponseDto> Items { get; set; } = new();
    public decimal TotalPrice { get; set; }
}