namespace SimpleShop.Api.Models;

public class CartResponseDTO
{
    public int UserId { get; set; }
    public List<CartItemResponseDto> Items { get; set; } = new();
    public decimal Total { get; set; }
}