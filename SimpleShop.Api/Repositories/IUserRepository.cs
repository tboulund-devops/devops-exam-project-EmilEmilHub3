using SimpleShop.Api.Models;

namespace SimpleShop.Api.Repositories;

/// <summary>
/// Defines data access operations for users.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Adds a new user to the data store.
    /// </summary>
    /// <param name="user">The user to add.</param>
    /// <returns>The created user.</returns>
    Task<User> AddAsync(User user);

    /// <summary>
    /// Gets a user by email address.
    /// </summary>
    /// <param name="email">The email address to search for.</param>
    /// <returns>The matching user if found; otherwise <c>null</c>.</returns>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// Gets a user by identifier.
    /// </summary>
    /// <param name="id">The identifier of the user.</param>
    /// <returns>The matching user if found; otherwise <c>null</c>.</returns>
    Task<User?> GetByIdAsync(int id);
}