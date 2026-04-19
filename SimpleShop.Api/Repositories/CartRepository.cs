using Microsoft.EntityFrameworkCore;
using SimpleShop.Api.Data;
using SimpleShop.Api.Models;

namespace SimpleShop.Api.Repositories;

/// <summary>
/// Repository responsible for data access operations related to cart items.
/// Encapsulates database interaction for creating, reading, updating,
/// deleting, and clearing shopping cart entries.
/// </summary>
public class CartRepository : ICartRepository
{
    private readonly AppDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="CartRepository"/> class.
    /// </summary>
    /// <param name="db">
    /// The database context used for cart data access.
    /// </param>
    public CartRepository(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Adds a new cart item to the database.
    /// After saving, the related product entity is explicitly loaded
    /// so the returned object contains product details.
    /// </summary>
    /// <param name="cartItem">
    /// The cart item to add.
    /// </param>
    /// <returns>
    /// The created cart item including its related product.
    /// </returns>
    public async Task<CartItem> AddAsync(CartItem cartItem)
    {
        _db.CartItems.Add(cartItem);
        await _db.SaveChangesAsync();

        // Load the related product before returning the created cart item.
        await _db.Entry(cartItem).Reference(c => c.Product).LoadAsync();
        return cartItem;
    }

    /// <summary>
    /// Gets all cart items belonging to a specific user.
    /// Product data is included to make the result ready for presentation.
    /// </summary>
    /// <param name="userId">
    /// The identifier of the user whose cart items should be returned.
    /// </param>
    /// <returns>
    /// A list of cart items for the specified user.
    /// </returns>
    public Task<List<CartItem>> GetByUserIdAsync(int userId)
    {
        return _db.CartItems
            .AsNoTracking()
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.Id)
            .ToListAsync();
    }

    /// <summary>
    /// Gets a single cart item by its identifier.
    /// Product data is included in the returned result.
    /// </summary>
    /// <param name="id">
    /// The identifier of the cart item.
    /// </param>
    /// <returns>
    /// The matching cart item if found; otherwise <c>null</c>.
    /// </returns>
    public Task<CartItem?> GetByIdAsync(int id)
    {
        return _db.CartItems
            .Include(c => c.Product)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    /// <summary>
    /// Updates an existing cart item in the database.
    /// After saving, the related product entity is explicitly loaded
    /// so the returned object contains product details.
    /// </summary>
    /// <param name="cartItem">
    /// The cart item to update.
    /// </param>
    /// <returns>
    /// The updated cart item including its related product.
    /// </returns>
    public async Task<CartItem> UpdateAsync(CartItem cartItem)
    {
        _db.CartItems.Update(cartItem);
        await _db.SaveChangesAsync();

        // Load the related product before returning the updated cart item.
        await _db.Entry(cartItem).Reference(c => c.Product).LoadAsync();
        return cartItem;
    }

    /// <summary>
    /// Deletes a specific cart item from the database.
    /// </summary>
    /// <param name="cartItem">
    /// The cart item to delete.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous delete operation.
    /// </returns>
    public async Task DeleteAsync(CartItem cartItem)
    {
        _db.CartItems.Remove(cartItem);
        await _db.SaveChangesAsync();
    }

    /// <summary>
    /// Removes all cart items belonging to a specific user.
    /// Used when a cart needs to be cleared, for example after checkout.
    /// </summary>
    /// <param name="userId">
    /// The identifier of the user whose cart should be cleared.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous clear operation.
    /// </returns>
    public async Task ClearByUserIdAsync(int userId)
    {
        var cartItems = await _db.CartItems
            .Where(c => c.UserId == userId)
            .ToListAsync();

        // Stop early if the user has no cart items.
        if (cartItems.Count == 0)
            return;

        _db.CartItems.RemoveRange(cartItems);
        await _db.SaveChangesAsync();
    }
}