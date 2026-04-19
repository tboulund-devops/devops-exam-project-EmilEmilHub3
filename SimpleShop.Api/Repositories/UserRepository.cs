using Microsoft.EntityFrameworkCore;
using SimpleShop.Api.Data;
using SimpleShop.Api.Models;

namespace SimpleShop.Api.Repositories;

/// <summary>
/// Repository responsible for data access operations related to users.
/// Encapsulates persistence and retrieval of user entities.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserRepository"/> class.
    /// </summary>
    /// <param name="db">The database context used for user data access.</param>
    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Adds a new user to the database.
    /// </summary>
    /// <param name="user">The user to add.</param>
    /// <returns>The created user.</returns>
    public async Task<User> AddAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    /// <summary>
    /// Gets a user by email address.
    /// </summary>
    /// <param name="email">The email address to search for.</param>
    /// <returns>The matching user if found; otherwise <c>null</c>.</returns>
    public Task<User?> GetByEmailAsync(string email)
    {
        return _db.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    /// <summary>
    /// Gets a user by identifier.
    /// </summary>
    /// <param name="id">The identifier of the user.</param>
    /// <returns>The matching user if found; otherwise <c>null</c>.</returns>
    public Task<User?> GetByIdAsync(int id)
    {
        return _db.Users.FindAsync(id).AsTask();
    }
}