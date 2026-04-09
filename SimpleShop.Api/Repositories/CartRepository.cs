using Microsoft.EntityFrameworkCore;
using SimpleShop.Api.Data;
using SimpleShop.Api.Models;

namespace SimpleShop.Api.Repositories;

public class CartRepository : ICartRepository
{
    private readonly AppDbContext _db;

    public CartRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<CartItem> AddAsync(CartItem cartItem)
    {
        _db.CartItems.Add(cartItem);
        await _db.SaveChangesAsync();

        await _db.Entry(cartItem).Reference(c => c.Product).LoadAsync();
        return cartItem;
    }

    public Task<List<CartItem>> GetByUserIdAsync(int userId)
    {
        return _db.CartItems
            .AsNoTracking()
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.Id)
            .ToListAsync();
    }

    public Task<CartItem?> GetByIdAsync(int id)
    {
        return _db.CartItems
            .Include(c => c.Product)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<CartItem> UpdateAsync(CartItem cartItem)
    {
        _db.CartItems.Update(cartItem);
        await _db.SaveChangesAsync();

        await _db.Entry(cartItem).Reference(c => c.Product).LoadAsync();
        return cartItem;
    }

    public async Task DeleteAsync(CartItem cartItem)
    {
        _db.CartItems.Remove(cartItem);
        await _db.SaveChangesAsync();
    }

    public async Task ClearByUserIdAsync(int userId)
    {
        var cartItems = await _db.CartItems
            .Where(c => c.UserId == userId)
            .ToListAsync();

        if (cartItems.Count == 0)
            return;

        _db.CartItems.RemoveRange(cartItems);
        await _db.SaveChangesAsync();
    }
}