namespace SimpleShop.Api.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public List<CartItem> CartItems { get; set; } = new();
    public List<Order> Orders { get; set; } = new();
}