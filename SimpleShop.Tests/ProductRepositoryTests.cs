using Microsoft.EntityFrameworkCore;
using SimpleShop.Api.Data;
using SimpleShop.Api.Models;
using SimpleShop.Api.Repositories;

namespace SimpleShop.Tests;

public class ProductRepositoryTests
{
    private static AppDbContext CreateDb()
    {
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
        var product = new Product { Name = "Milk", Price = 12.5m };

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

        db.Products.Add(new Product { Id = 2, Name = "B", Price = 2m });
        db.Products.Add(new Product { Id = 1, Name = "A", Price = 1m });
        await db.SaveChangesAsync();

        var repo = new ProductRepository(db);

        // Act
        var result = await repo.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].Id);
        Assert.Equal(2, result[1].Id);
    }
}