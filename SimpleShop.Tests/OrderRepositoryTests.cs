using Microsoft.EntityFrameworkCore;
using SimpleShop.Api.Data;
using SimpleShop.Api.Models;
using SimpleShop.Api.Repositories;

namespace SimpleShop.Tests;

public class OrderRepositoryTests
{
    private static AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddAsync_WhenCalled_PersistsOrderAndOrderLines_AndLoadsProducts()
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
            new Product
            {
                Id = 10,
                Name = "Milk",
                Price = 10m
            },
            new Product
            {
                Id = 20,
                Name = "Bread",
                Price = 25m
            }
        );

        await db.SaveChangesAsync();

        var repo = new OrderRepository(db);

        var order = new Order
        {
            UserId = 1,
            OrderLines = new List<OrderLine>
            {
                new()
                {
                    ProductId = 10,
                    Quantity = 2,
                    UnitPrice = 10m
                },
                new()
                {
                    ProductId = 20,
                    Quantity = 1,
                    UnitPrice = 25m
                }
            }
        };

        var created = await repo.AddAsync(order);

        Assert.True(created.Id > 0);
        Assert.Equal(1, created.UserId);
        Assert.Equal(2, created.OrderLines.Count);

        Assert.All(created.OrderLines, line => Assert.True(line.Id > 0));
        Assert.All(created.OrderLines, line => Assert.NotNull(line.Product));

        Assert.Contains(created.OrderLines, l => l.Product!.Name == "Milk");
        Assert.Contains(created.OrderLines, l => l.Product!.Name == "Bread");

        Assert.True(await db.Orders.AnyAsync(o => o.Id == created.Id));
        Assert.Equal(2, await db.OrderLines.CountAsync(ol => ol.OrderId == created.Id));
    }
}