using Moq;
using SimpleShop.Api.Models;
using SimpleShop.Api.Repositories;
using SimpleShop.Api.Services;

namespace SimpleShop.Tests;

public class ProductServiceTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateAsync_WhenNameIsMissing_ThrowsArgumentException(string name)
    {
        // Arrange
        var repo = new Mock<IProductRepository>();
        var service = new ProductService(repo.Object);
        var dto = new CreateProductDto { Name = name, Price = 10m };

        // Act
        var act = async () => await service.CreateAsync(dto);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
        repo.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenPriceIsNegative_ThrowsArgumentException()
    {
        // Arrange
        var repo = new Mock<IProductRepository>();
        var service = new ProductService(repo.Object);
        var dto = new CreateProductDto { Name = "Milk", Price = -1m };

        // Act
        var act = async () => await service.CreateAsync(dto);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
        repo.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenValid_TrimsName_AndCallsRepository()
    {
        // Arrange
        var repo = new Mock<IProductRepository>();

        repo.Setup(r => r.AddAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) =>
            {
                p.Id = 1;
                return p;
            });

        var service = new ProductService(repo.Object);
        var dto = new CreateProductDto { Name = "  Milk  ", Price = 12.5m };

        // Act
        var created = await service.CreateAsync(dto);

        // Assert
        Assert.Equal(1, created.Id);
        Assert.Equal("Milk", created.Name);
        Assert.Equal(12.5m, created.Price);

        repo.Verify(r => r.AddAsync(It.Is<Product>(p =>
            p.Name == "Milk" &&
            p.Price == 12.5m &&
            p.Id == 0   // før repo “giver” Id, skal den typisk være default
        )), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsListFromRepository()
    {
        // Arrange
        var repo = new Mock<IProductRepository>();
        var expected = new List<Product>
        {
            new() { Id = 1, Name = "Milk", Price = 10m },
            new() { Id = 2, Name = "Bread", Price = 20m }
        };

        repo.Setup(r => r.GetAllAsync()).ReturnsAsync(expected);

        var service = new ProductService(repo.Object);

        // Act
        var result = await service.GetAllAsync();

        // Assert
        Assert.Same(expected, result);
        repo.Verify(r => r.GetAllAsync(), Times.Once);
    }
}