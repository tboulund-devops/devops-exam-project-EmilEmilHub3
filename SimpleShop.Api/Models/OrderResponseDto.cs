namespace SimpleShop.Api.Models;

public class OrderResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public List<OrderLineResponseDto> OrderLines { get; set; } = new();
    public decimal TotalPrice { get; set; }
}