using Microsoft.EntityFrameworkCore;
using SimpleShop.Api.Data;
using SimpleShop.Api.Models;
using SimpleShop.Api.Repositories;

namespace SimpleShop.Tests;

public class ProductRepositoryTests
{
    private static AppDbContext CreateDb()
    {
        // Create a fresh in-memory database for each test
        // (Unique DB name avoids cross-test state bleed)
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddAsync_WhenCalled_PersistsProductAndReturnsIt()
    {
        // Arrange
        await using var db = CreateDb();
        var repo = new ProductRepository(db);

        var product = new Product
        {
            Name = "Milk",
            Price = 12.5m
        };

        // Act
        var created = await repo.AddAsync(product);

        // Assert
        Assert.True(created.Id > 0);
        Assert.Equal("Milk", created.Name);

        var count = await db.Products.CountAsync();
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task GetAllAsync_WhenMultipleProducts_ReturnsOrderedById()
    {
        // Arrange
        await using var db = CreateDb();
        var repo = new ProductRepository(db);

        db.Products.Add(new Product { Id = 2, Name = "Bread", Price = 2m });
        db.Products.Add(new Product { Id = 1, Name = "Milk", Price = 1m });
        await db.SaveChangesAsync();

        // Act
        var result = await repo.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].Id);
        Assert.Equal(2, result[1].Id);
    }

    [Fact]
    public async Task GetAllAsync_WithSearch_ReturnsOnlyMatchingProducts()
    {
        // Arrange
        await using var db = CreateDb();
        var repo = new ProductRepository(db);

        db.Products.Add(new Product { Id = 1, Name = "Milk", Price = 10m });
        db.Products.Add(new Product { Id = 2, Name = "Chocolate Milk", Price = 15m });
        db.Products.Add(new Product { Id = 3, Name = "Bread", Price = 20m });
        await db.SaveChangesAsync();

        // Act
        var result = await repo.GetAllAsync("milk");

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, p => Assert.Contains("milk", p.Name, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task GetAllAsync_WithEmptySearch_ReturnsAllProducts()
    {
        // Arrange
        await using var db = CreateDb();
        var repo = new ProductRepository(db);

        db.Products.Add(new Product { Id = 1, Name = "Milk", Price = 10m });
        db.Products.Add(new Product { Id = 2, Name = "Bread", Price = 20m });
        await db.SaveChangesAsync();

        // Act
        var result = await repo.GetAllAsync("");

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsProduct()
    {
        // Arrange
        await using var db = CreateDb();
        var repo = new ProductRepository(db);

        db.Products.Add(new Product { Id = 1, Name = "Milk", Price = 10m });
        await db.SaveChangesAsync();

        // Act
        var result = await repo.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result!.Id);
        Assert.Equal("Milk", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotExists_ReturnsNull()
    {
        // Arrange
        await using var db = CreateDb();
        var repo = new ProductRepository(db);

        // Act
        var result = await repo.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_WhenCalled_UpdatesProduct()
    {
        // Arrange
        await using var db = CreateDb();
        var repo = new ProductRepository(db);

        db.Products.Add(new Product { Id = 1, Name = "Milk", Price = 10m });
        await db.SaveChangesAsync();

        var tracked = await db.Products.FindAsync(1);
        Assert.NotNull(tracked);

        tracked!.Name = "Milk 2";
        tracked.Price = 20m;

        // Act
        var updated = await repo.UpdateAsync(tracked);

        // Assertt
        Assert.Equal(1, updated.Id);
        Assert.Equal("Milk 2", updated.Name);
        Assert.Equal(20m, updated.Price);

        var fromDb = await db.Products.FindAsync(1);
        Assert.NotNull(fromDb);
        Assert.Equal("Milk 2", fromDb!.Name);
        Assert.Equal(20m, fromDb.Price);
    }

    [Fact]
    public async Task DeleteAsync_WhenCalled_RemovesProductFromDatabase()
    {
        // Arrange
        await using var db = CreateDb();
        var repo = new ProductRepository(db);

        db.Products.Add(new Product { Id = 1, Name = "Milk", Price = 10m });
        await db.SaveChangesAsync();

        var product = await db.Products.FindAsync(1);
        Assert.NotNull(product);

        // Act
        await repo.DeleteAsync(product!);

        // Assert
        var fromDb = await db.Products.FindAsync(1);
        Assert.Null(fromDb);
        Assert.Equal(0, await db.Products.CountAsync());
    }
}
