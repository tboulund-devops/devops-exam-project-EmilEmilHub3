using SimpleShop.Api.Models;

namespace SimpleShop.Api.Repositories;

public interface IUserRepository
{
    Task<User> AddAsync(User user);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(int id);
}