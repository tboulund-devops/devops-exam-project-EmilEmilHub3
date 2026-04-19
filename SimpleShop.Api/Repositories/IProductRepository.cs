using SimpleShop.Api.Models;

namespace SimpleShop.Api.Repositories;

/// <summary>
/// Defines data access operations for products.
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Gets all products, optionally filtered by a search term.
    /// </summary>
    /// <param name="search">Optional search term used to filter products.</param>
    /// <returns>A list of matching products.</returns>
    Task<List<Product>> GetAllAsync(string? search = null);

    /// <summary>
    /// Adds a new product to the data store.
    /// </summary>
    /// <param name="product">The product to add.</param>
    /// <returns>The created product.</returns>
    Task<Product> AddAsync(Product product);

    /// <summary>
    /// Gets a product by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the product.</param>
    /// <returns>The matching product if found; otherwise <c>null</c>.</returns>
    Task<Product?> GetByIdAsync(int id);

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="product">The product to update.</param>
    /// <returns>The updated product.</returns>
    Task<Product> UpdateAsync(Product product);

    /// <summary>
    /// Deletes a specific product.
    /// </summary>
    /// <param name="product">The product to delete.</param>
    Task DeleteAsync(Product product);
}