using SimpleShop.Api.Models;

namespace SimpleShop.Api.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync(string? search = null);
    Task<Product> AddAsync(Product product);
    Task<Product?> GetByIdAsync(int id);
    Task<Product> UpdateAsync(Product product);
    Task DeleteAsync(Product product);
}
