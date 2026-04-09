using SimpleShop.Api.Models;

namespace SimpleShop.Api.Repositories;

public interface IOrderRepository
{
    Task<Order> AddAsync(Order order);
}