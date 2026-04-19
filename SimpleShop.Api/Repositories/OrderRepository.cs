using Microsoft.EntityFrameworkCore;
using SimpleShop.Api.Data;
using SimpleShop.Api.Models;

namespace SimpleShop.Api.Repositories;

/// <summary>
/// Repository responsible for data access operations related to orders.
/// Handles persistence of orders and loading of related order details.
/// </summary>
public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderRepository"/> class.
    /// </summary>
    /// <param name="db">The database context used for order data access.</param>
    public OrderRepository(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Adds a new order to the database.
    /// After saving, the order is reloaded with related order lines
    /// and product data to return a complete result.
    /// </summary>
    /// <param name="order">The order to add.</param>
    /// <returns>The created order including its related order lines and products.</returns>
    public async Task<Order> AddAsync(Order order)
    {
        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        // Reload the saved order with related data for a complete response object.
        return await _db.Orders
            .AsNoTracking()
            .Include(o => o.OrderLines)
            .ThenInclude(ol => ol.Product)
            .FirstAsync(o => o.Id == order.Id);
    }
}