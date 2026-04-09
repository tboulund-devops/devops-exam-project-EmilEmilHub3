namespace SimpleShop.Api.Models;

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }

    public User? User { get; set; }
    public List<OrderLine> OrderLines { get; set; } = new();
}