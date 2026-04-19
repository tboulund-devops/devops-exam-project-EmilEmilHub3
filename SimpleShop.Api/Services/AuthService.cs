using System.Security.Cryptography;
using System.Text;
using SimpleShop.Api.Models;
using SimpleShop.Api.Repositories;

namespace SimpleShop.Api.Services;

/// <summary>
/// Provides authentication-related business logic.
/// Responsible for user registration, login validation,
/// and password hashing.
/// </summary>
public class AuthService
{
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthService"/> class.
    /// </summary>
    /// <param name="userRepository">
    /// Repository used for user persistence and lookup.
    /// </param>
    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <summary>
    /// Registers a new user after validating the input
    /// and ensuring the email address is unique.
    /// </summary>
    /// <param name="dto">
    /// The registration data provided by the client.
    /// </param>
    /// <returns>
    /// The created user.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when required fields are missing
    /// or a user with the same email already exists.
    /// </exception>
    public async Task<User> RegisterAsync(RegisterUserDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username))
            throw new ArgumentException("Username is required.", nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Email))
            throw new ArgumentException("Email is required.", nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Password))
            throw new ArgumentException("Password is required.", nameof(dto));

        // Normalize email before lookup and storage to avoid duplicates caused by casing or whitespace.
        var normalizedEmail = dto.Email.Trim().ToLower();
        var existingUser = await _userRepository.GetByEmailAsync(normalizedEmail);

        if (existingUser is not null)
            throw new ArgumentException("A user with that email already exists.", nameof(dto));

        var user = new User
        {
            Username = dto.Username.Trim(),
            Email = normalizedEmail,
            PasswordHash = HashPassword(dto.Password)
        };

        return await _userRepository.AddAsync(user);
    }

    /// <summary>
    /// Validates login credentials for an existing user.
    /// </summary>
    /// <param name="dto">
    /// The login data provided by the client.
    /// </param>
    /// <returns>
    /// The matching user if authentication succeeds; otherwise <c>null</c>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when required login fields are missing.
    /// </exception>
    public async Task<User?> LoginAsync(LoginDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
            throw new ArgumentException("Email is required.", nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Password))
            throw new ArgumentException("Password is required.", nameof(dto));

        // Normalize email before lookup to match stored user format.
        var normalizedEmail = dto.Email.Trim().ToLower();
        var user = await _userRepository.GetByEmailAsync(normalizedEmail);

        if (user is null)
            return null;

        var passwordHash = HashPassword(dto.Password);
        return user.PasswordHash == passwordHash ? user : null;
    }

    /// <summary>
    /// Hashes a plaintext password using SHA256.
    /// </summary>
    /// <param name="password">
    /// The plaintext password to hash.
    /// </param>
    /// <returns>
    /// The hexadecimal representation of the hashed password.
    /// </returns>
    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }
}