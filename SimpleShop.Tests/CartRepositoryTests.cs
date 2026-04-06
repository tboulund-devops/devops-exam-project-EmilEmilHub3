using Microsoft.EntityFrameworkCore;
using SimpleShop.Api.Data;
using SimpleShop.Api.Models;
using SimpleShop.Api.Repositories;

namespace SimpleShop.Tests;

public class CartRepositoryTests
{
    private static AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddAsync_WhenCalled_PersistsCartItem_AndLoadsProduct()
    {
        await using var db = CreateDb();

        db.Users.Add(new User
        {
            Id = 1,
            Username = "Emil",
            Email = "test@test.com",
            PasswordHash = "HASH"
        });

        db.Products.Add(new Product
        {
            Id = 2,
            Name = "Milk",
            Price = 10m
        });

        await db.SaveChangesAsync();

        var repo = new CartRepository(db);

        var cartItem = new CartItem
        {
            UserId = 1,
            ProductId = 2,
            Quantity = 3
        };

        var created = await repo.AddAsync(cartItem);

        Assert.True(created.Id > 0);
        Assert.NotNull(created.Product);
        Assert.Equal("Milk", created.Product!.Name);
    }

    [Fact]
    public async Task GetByUserIdAsync_WhenUserHasItems_ReturnsOrderedItemsWithProducts()
    {
        await using var db = CreateDb();

        db.Users.Add(new User
        {
            Id = 1,
            Username = "Emil",
            Email = "test@test.com",
            PasswordHash = "HASH"
        });

        db.Products.AddRange(
            new Product { Id = 10, Name = "Milk", Price = 10m },
            new Product { Id = 20, Name = "Bread", Price = 20m }
        );

        db.CartItems.AddRange(
            new CartItem { Id = 2, UserId = 1, ProductId = 20, Quantity = 1 },
            new CartItem { Id = 1, UserId = 1, ProductId = 10, Quantity = 2 }
        );

        await db.SaveChangesAsync();

        var repo = new CartRepository(db);

        var result = await repo.GetByUserIdAsync(1);

        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].Id);
        Assert.Equal(2, result[1].Id);
        Assert.NotNull(result[0].Product);
        Assert.NotNull(result[1].Product);
    }
}