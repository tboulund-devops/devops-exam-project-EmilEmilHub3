using SimpleShop.Api.Models;

namespace SimpleShop.Api.Repositories;

public interface ICartRepository
{
    Task<CartItem> AddAsync(CartItem cartItem);
    Task<List<CartItem>> GetByUserIdAsync(int userId);
    Task<CartItem?> GetByIdAsync(int id);
    Task<CartItem> UpdateAsync(CartItem cartItem);
    Task DeleteAsync(CartItem cartItem);
}