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

    public Task<Product?> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Id must be greater than 0.", nameof(id));

        return _repo.GetByIdAsync(id);
    }

    public Task<Product> CreateAsync(CreateProductDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Name is required.", nameof(dto));

        if (dto.Price < 0)
            throw new ArgumentException("Price cannot be negative.", nameof(dto));

        var product = new Product
        {
            Name = dto.Name.Trim(),
            Price = dto.Price.HasValue ? dto.Price.Value : 0
        };

        return _repo.AddAsync(product);
    }

    public async Task<Product?> UpdateAsync(int id, CreateProductDto dto)
    {
        if (id <= 0)
            throw new ArgumentException("Id must be greater than 0.", nameof(id));

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Name is required.", nameof(dto));

        if (dto.Price < 0)
            throw new ArgumentException("Price cannot be negative.", nameof(dto));

        var existing = await _repo.GetByIdAsync(id);
        if (existing is null)
            return null;

        existing.Name = dto.Name.Trim();
        existing.Price = dto.Price.HasValue ? dto.Price.Value : 0;

        return await _repo.UpdateAsync(existing);
    }
}