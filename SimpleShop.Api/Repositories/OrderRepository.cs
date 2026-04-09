using Microsoft.EntityFrameworkCore;
using SimpleShop.Api.Data;
using SimpleShop.Api.Models;

namespace SimpleShop.Api.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _db;

    public OrderRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Order> AddAsync(Order order)
    {
        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        return await _db.Orders
            .AsNoTracking()
            .Include(o => o.OrderLines)
            .ThenInclude(ol => ol.Product)
            .FirstAsync(o => o.Id == order.Id);
    }
}