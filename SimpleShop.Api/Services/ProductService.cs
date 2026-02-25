using SimpleShop.Api.Models;
using SimpleShop.Api.Repositories;

namespace SimpleShop.Api.Services;

public class ProductService
{
    private readonly IProductRepository _repo;

    public ProductService(IProductRepository repo)
    {
        _repo = repo;
    }

    public Task<List<Product>> GetAllAsync() => _repo.GetAllAsync();

    public Task<Product> CreateAsync(CreateProductDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Name is required.", nameof(dto.Name));

        if (dto.Price < 0)
            throw new ArgumentException("Price cannot be negative.", nameof(dto.Price));

        var product = new Product
        {
            Name = dto.Name.Trim(),
            Price = dto.Price
        };

        return _repo.AddAsync(product);
    }
}