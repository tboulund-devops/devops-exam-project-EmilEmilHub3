using SimpleShop.Api.Models;

namespace SimpleShop.Api.Repositories;

/// <summary>
/// Defines data access operations for orders.
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// Adds a new order to the data store.
    /// </summary>
    /// <param name="order">The order to add.</param>
    /// <returns>The created order.</returns>
    Task<Order> AddAsync(Order order);
}