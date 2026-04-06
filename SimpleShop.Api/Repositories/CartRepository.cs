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
}