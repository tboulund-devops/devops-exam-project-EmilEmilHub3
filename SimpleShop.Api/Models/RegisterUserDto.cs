namespace SimpleShop.Api.Models;

/// <summary>
/// Data transfer object used to register a new user.
/// </summary>
public class RegisterUserDto
{
    /// <summary>
    /// Gets or sets the username chosen by the user.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's plaintext password.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}