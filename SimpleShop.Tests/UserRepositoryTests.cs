using Microsoft.EntityFrameworkCore;
using SimpleShop.Api.Data;
using SimpleShop.Api.Models;
using SimpleShop.Api.Repositories;

namespace SimpleShop.Tests;

public class UserRepositoryTests
{
    private static AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddAsync_WhenCalled_PersistsUser()
    {
        await using var db = CreateDb();
        var repo = new UserRepository(db);

        var user = new User
        {
            Username = "Emil",
            Email = "test@test.com",
            PasswordHash = "HASH"
        };

        var created = await repo.AddAsync(user);

        Assert.True(created.Id > 0);
        Assert.Equal(1, await db.Users.CountAsync());
    }

    [Fact]
    public async Task GetByEmailAsync_WhenExists_ReturnsUser()
    {
        await using var db = CreateDb();
        db.Users.Add(new User
        {
            Id = 1,
            Username = "Emil",
            Email = "test@test.com",
            PasswordHash = "HASH"
        });
        await db.SaveChangesAsync();

        var repo = new UserRepository(db);

        var result = await repo.GetByEmailAsync("test@test.com");

        Assert.NotNull(result);
        Assert.Equal(1, result!.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsUser()
    {
        await using var db = CreateDb();
        db.Users.Add(new User
        {
            Id = 1,
            Username = "Emil",
            Email = "test@test.com",
            PasswordHash = "HASH"
        });
        await db.SaveChangesAsync();

        var repo = new UserRepository(db);

        var result = await repo.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Emil", result!.Username);
    }
}