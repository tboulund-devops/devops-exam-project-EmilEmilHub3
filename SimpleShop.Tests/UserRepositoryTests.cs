using Microsoft.EntityFrameworkCore;
using SimpleShop.Api.Data;
using SimpleShop.Api.Models;
using SimpleShop.Api.Repositories;

namespace SimpleShop.Tests;

/// <summary>
/// Repository tests for <see cref="UserRepository"/>.
/// These tests use EF Core InMemoryDatabase to verify
/// user persistence and lookup behavior.
/// Tests follow the Arrange, Act, Assert (AAA) pattern.
/// </summary>
public class UserRepositoryTests
{
    private static AppDbContext CreateDb()
    {
        // Create a fresh in-memory database for each test
        // to ensure isolation between test cases.
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddAsync_WhenCalled_PersistsUser()
    {
        // Arrange
        await using var db = CreateDb();
        var repo = new UserRepository(db);

        var user = new User
        {
            Username = "Emil",
            Email = "test@test.com",
            PasswordHash = "HASH"
        };

        // Act
        var created = await repo.AddAsync(user);

        // Assert
        Assert.True(created.Id > 0);
        Assert.Equal(1, await db.Users.CountAsync());
    }

    [Fact]
    public async Task GetByEmailAsync_WhenExists_ReturnsUser()
    {
        // Arrange
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

        // Act
        var result = await repo.GetByEmailAsync("test@test.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result!.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsUser()
    {
        // Arrange
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

        // Act
        var result = await repo.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Emil", result!.Username);
    }
}