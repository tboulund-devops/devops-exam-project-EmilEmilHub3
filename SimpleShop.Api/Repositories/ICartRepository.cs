using SimpleShop.Api.Models;

namespace SimpleShop.Api.Repositories;

/// <summary>
/// Defines data access operations for shopping cart items.
/// </summary>
public interface ICartRepository
{
    /// <summary>
    /// Adds a new cart item to the data store.
    /// </summary>
    /// <param name="cartItem">The cart item to add.</param>
    /// <returns>The created cart item.</returns>
    Task<CartItem> AddAsync(CartItem cartItem);

    /// <summary>
    /// Gets all cart items belonging to a specific user.
    /// </summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <returns>A list of cart items for the specified user.</returns>
    Task<List<CartItem>> GetByUserIdAsync(int userId);

    /// <summary>
    /// Gets a cart item by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the cart item.</param>
    /// <returns>The matching cart item if found; otherwise <c>null</c>.</returns>
    Task<CartItem?> GetByIdAsync(int id);

    /// <summary>
    /// Updates an existing cart item.
    /// </summary>
    /// <param name="cartItem">The cart item to update.</param>
    /// <returns>The updated cart item.</returns>
    Task<CartItem> UpdateAsync(CartItem cartItem);

    /// <summary>
    /// Deletes a specific cart item.
    /// </summary>
    /// <param name="cartItem">The cart item to delete.</param>
    Task DeleteAsync(CartItem cartItem);

    /// <summary>
    /// Removes all cart items belonging to a specific user.
    /// </summary>
    /// <param name="userId">The identifier of the user whose cart should be cleared.</param>
    Task ClearByUserIdAsync(int userId);
}