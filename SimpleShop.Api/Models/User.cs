namespace SimpleShop.Api.Models;

/// <summary>
/// Represents a registered user in the SimpleShop application.
/// </summary>
public class User
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the username of the user.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address of the user.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the hashed password of the user.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the cart items belonging to the user.
    /// </summary>
    public List<CartItem> CartItems { get; set; } = new();

    /// <summary>
    /// Gets or sets the orders placed by the user.
    /// </summary>
    public List<Order> Orders { get; set; } = new();
}