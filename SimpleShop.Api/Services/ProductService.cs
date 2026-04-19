using SimpleShop.Api.Models;
using SimpleShop.Api.Repositories;

namespace SimpleShop.Api.Services;

/// <summary>
/// Provides business logic related to products.
/// Responsible for validating input data and coordinating
/// repository operations for product management.
/// </summary>
public class ProductService
{
    private readonly IProductRepository _repo;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductService"/> class.
    /// </summary>
    /// <param name="repo">
    /// Repository used for product data access.
    /// </param>
    public ProductService(IProductRepository repo)
    {
        _repo = repo;
    }

    /// <summary>
    /// Gets all products, optionally filtered by a search term.
    /// </summary>
    /// <param name="search">
    /// Optional search term used to filter products.
    /// </param>
    /// <returns>
    /// A list of matching products.
    /// </returns>
    public Task<List<Product>> GetAllAsync(string? search = null)
    {
        return _repo.GetAllAsync(search);
    }

    /// <summary>
    /// Gets a product by its identifier.
    /// </summary>
    /// <param name="id">
    /// The identifier of the product.
    /// </param>
    /// <returns>
    /// The matching product if found; otherwise <c>null</c>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the identifier is not greater than zero.
    /// </exception>
    public Task<Product?> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Id must be greater than 0.", nameof(id));

        return _repo.GetByIdAsync(id);
    }

    /// <summary>
    /// Creates a new product after validating the input.
    /// </summary>
    /// <param name="dto">
    /// The product creation data.
    /// </param>
    /// <returns>
    /// The created product.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when required fields are missing
    /// or the price is invalid.
    /// </exception>
    public Task<Product> CreateAsync(CreateProductDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Name is required.", nameof(dto));

        if (!dto.Price.HasValue || dto.Price.Value < 0)
            throw new ArgumentException("Price must be provided and cannot be negative.", nameof(dto));

        var product = new Product
        {
            Name = dto.Name.Trim(),
            Price = dto.Price.Value
        };

        return _repo.AddAsync(product);
    }

    /// <summary>
    /// Updates an existing product after validating the input.
    /// </summary>
    /// <param name="id">
    /// The identifier of the product to update.
    /// </param>
    /// <param name="dto">
    /// The updated product data.
    /// </param>
    /// <returns>
    /// The updated product if found; otherwise <c>null</c>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the identifier or product data is invalid.
    /// </exception>
    public async Task<Product?> UpdateAsync(int id, CreateProductDto dto)
    {
        if (id <= 0)
            throw new ArgumentException("Id must be greater than 0.", nameof(id));

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Name is required.", nameof(dto));

        if (!dto.Price.HasValue || dto.Price.Value < 0)
            throw new ArgumentException("Price must be provided and cannot be negative.", nameof(dto));

        var existing = await _repo.GetByIdAsync(id);
        if (existing is null)
            return null;

        existing.Name = dto.Name.Trim();
        existing.Price = dto.Price.Value;

        return await _repo.UpdateAsync(existing);
    }

    /// <summary>
    /// Deletes a product by its identifier.
    /// </summary>
    /// <param name="id">
    /// The identifier of the product to delete.
    /// </param>
    /// <returns>
    /// <c>true</c> if the product was found and deleted;
    /// otherwise <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the identifier is not greater than zero.
    /// </exception>
    public async Task<bool> DeleteAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Id must be greater than 0.", nameof(id));

        var existing = await _repo.GetByIdAsync(id);
        if (existing is null)
            return false;

        await _repo.DeleteAsync(existing);
        return true;
    }
}