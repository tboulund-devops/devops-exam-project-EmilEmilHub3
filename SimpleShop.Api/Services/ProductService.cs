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

    public Task<List<Product>> GetAllAsync(string? search = null)
    {
        return _repo.GetAllAsync(search);
    }

    public Task<Product?> GetByIdAsync(int id)
    {
        ValidateId(id);
        return _repo.GetByIdAsync(id);
    }

    public Task<Product> CreateAsync(CreateProductDto dto)
    {
        ValidateProductDto(dto);

        var product = new Product
        {
            Name = dto.Name.Trim(),
            Price = dto.Price!.Value
        };

        return _repo.AddAsync(product);
    }

    public async Task<Product?> UpdateAsync(int id, CreateProductDto dto)
    {
        ValidateId(id);
        ValidateProductDto(dto);

        var existing = await _repo.GetByIdAsync(id);
        if (existing is null)
            return null;

        existing.Name = dto.Name.Trim();
        existing.Price = dto.Price!.Value;

        return await _repo.UpdateAsync(existing);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        ValidateId(id);

        var existing = await _repo.GetByIdAsync(id);
        if (existing is null)
            return false;

        await _repo.DeleteAsync(existing);
        return true;
    }

    private static void ValidateId(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Id must be greater than 0.", nameof(id));
    }

    private static void ValidateProductDto(CreateProductDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Name is required.", nameof(dto));

        if (!dto.Price.HasValue || dto.Price.Value < 0)
            throw new ArgumentException("Price must be provided and cannot be negative.", nameof(dto));
    }
}