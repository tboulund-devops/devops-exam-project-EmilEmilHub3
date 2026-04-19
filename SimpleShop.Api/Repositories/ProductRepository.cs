using Microsoft.EntityFrameworkCore;
using SimpleShop.Api.Data;
using SimpleShop.Api.Models;

namespace SimpleShop.Api.Repositories;

/// <summary>
/// Repository responsible for data access operations related to products.
/// Encapsulates database interaction for creating, reading,
/// updating, deleting, and searching products.
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductRepository"/> class.
    /// </summary>
    /// <param name="db">The database context used for product data access.</param>
    public ProductRepository(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Gets all products, optionally filtered by a search term.
    /// Search is performed against the product name.
    /// </summary>
    /// <param name="search">Optional search term used to filter product names.</param>
    /// <returns>A list of matching products ordered by identifier.</returns>
    public Task<List<Product>> GetAllAsync(string? search = null)
    {
        var query = _db.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim();

            query = query.Where(p =>
                EF.Functions.Like(p.Name, $"%{normalizedSearch}%"));
        }

        return query.OrderBy(p => p.Id).ToListAsync();
    }

    /// <summary>
    /// Adds a new product to the database.
    /// </summary>
    /// <param name="product">The product to add.</param>
    /// <returns>The created product.</returns>
    public async Task<Product> AddAsync(Product product)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return product;
    }

    /// <summary>
    /// Gets a product by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the product.</param>
    /// <returns>The matching product if found; otherwise <c>null</c>.</returns>
    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _db.Products.FindAsync(id);
    }

    /// <summary>
    /// Updates an existing product in the database.
    /// </summary>
    /// <param name="product">The product to update.</param>
    /// <returns>The updated product.</returns>
    public async Task<Product> UpdateAsync(Product product)
    {
        _db.Products.Update(product);
        await _db.SaveChangesAsync();
        return product;
    }

    /// <summary>
    /// Deletes a specific product from the database.
    /// </summary>
    /// <param name="product">The product to delete.</param>
    public async Task DeleteAsync(Product product)
    {
        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
    }
}