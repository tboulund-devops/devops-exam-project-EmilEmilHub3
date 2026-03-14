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

        Product? captured = null;

        repo.Setup(r => r.AddAsync(It.IsAny<Product>()))
            .Callback<Product>(p => captured = p)
            .ReturnsAsync((Product p) => new Product
            {
                Id = 1,
                Name = p.Name,
                Price = p.Price
            });

        var service = new ProductService(repo.Object);
        var dto = new CreateProductDto { Name = "  Milk  ", Price = 12.5m };

        // Act
        var created = await service.CreateAsync(dto);

        // Assert
        Assert.Equal(1, created.Id);
        Assert.Equal("Milk", created.Name);
        Assert.Equal(12.5m, created.Price);

        repo.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Once);

        Assert.NotNull(captured);
        Assert.Equal("Milk", captured!.Name);
        Assert.Equal(12.5m, captured.Price);
        Assert.Equal(0, captured.Id);
    }

    [Fact]
    public async Task GetAllAsync_WithoutSearch_ReturnsListFromRepository()
    {
        // Arrange
        var repo = new Mock<IProductRepository>();
        var expected = new List<Product>
        {
            new() { Id = 1, Name = "Milk", Price = 10m },
            new() { Id = 2, Name = "Bread", Price = 20m }
        };

        repo.Setup(r => r.GetAllAsync(null)).ReturnsAsync(expected);

        var service = new ProductService(repo.Object);

        // Act
        var result = await service.GetAllAsync();

        // Assert
        Assert.Same(expected, result);
        repo.Verify(r => r.GetAllAsync(null), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WithSearch_ForwardsSearchToRepository()
    {
        // Arrange
        var repo = new Mock<IProductRepository>();
        var expected = new List<Product>
        {
            new() { Id = 1, Name = "Milk", Price = 10m }
        };

        repo.Setup(r => r.GetAllAsync("milk")).ReturnsAsync(expected);

        var service = new ProductService(repo.Object);

        // Act
        var result = await service.GetAllAsync("milk");

        // Assert
        Assert.Same(expected, result);
        repo.Verify(r => r.GetAllAsync("milk"), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenIdIsInvalid_ThrowsArgumentException()
    {
        // Arrange
        var repo = new Mock<IProductRepository>();
        var service = new ProductService(repo.Object);

        // Act
        var act = async () => await service.GetByIdAsync(0);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
        repo.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsProduct()
    {
        // Arrange
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Product { Id = 1, Name = "Milk", Price = 10m });

        var service = new ProductService(repo.Object);

        // Act
        var result = await service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result!.Id);
        repo.Verify(r => r.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenIdIsInvalid_ThrowsArgumentException()
    {
        // Arrange
        var repo = new Mock<IProductRepository>();
        var service = new ProductService(repo.Object);
        var dto = new CreateProductDto { Name = "Milk", Price = 10m };

        // Act
        var act = async () => await service.UpdateAsync(0, dto);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
        repo.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        repo.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenNameMissing_ThrowsArgumentException()
    {
        // Arrange
        var repo = new Mock<IProductRepository>();
        var service = new ProductService(repo.Object);
        var dto = new CreateProductDto { Name = "   ", Price = 10m };

        // Act
        var act = async () => await service.UpdateAsync(1, dto);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
        repo.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        repo.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenNotFound_ReturnsNull()
    {
        // Arrange
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Product?)null);

        var service = new ProductService(repo.Object);
        var dto = new CreateProductDto { Name = "Milk", Price = 10m };

        // Act
        var result = await service.UpdateAsync(1, dto);

        // Assert
        Assert.Null(result);
        repo.Verify(r => r.GetByIdAsync(1), Times.Once);
        repo.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenValid_TrimsName_AndCallsUpdate()
    {
        // Arrange
        var repo = new Mock<IProductRepository>();

        var existing = new Product { Id = 1, Name = "Old", Price = 1m };
        repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);

        Product? updatedCaptured = null;

        repo.Setup(r => r.UpdateAsync(It.IsAny<Product>()))
            .Callback<Product>(p => updatedCaptured = p)
            .ReturnsAsync((Product p) => p);

        var service = new ProductService(repo.Object);
        var dto = new CreateProductDto { Name = "  New Name  ", Price = 99m };

        // Act
        var result = await service.UpdateAsync(1, dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result!.Id);
        Assert.Equal("New Name", result.Name);
        Assert.Equal(99m, result.Price);

        repo.Verify(r => r.GetByIdAsync(1), Times.Once);
        repo.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Once);

        Assert.NotNull(updatedCaptured);
        Assert.Equal("New Name", updatedCaptured!.Name);
        Assert.Equal(99m, updatedCaptured.Price);
        Assert.Equal(1, updatedCaptured.Id);
    }

    [Fact]
    public async Task DeleteAsync_WhenIdIsInvalid_ThrowsArgumentException()
    {
        // Arrange
        var repo = new Mock<IProductRepository>();
        var service = new ProductService(repo.Object);

        // Act
        var act = async () => await service.DeleteAsync(0);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
        repo.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        repo.Verify(r => r.DeleteAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenProductDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Product?)null);

        var service = new ProductService(repo.Object);

        // Act
        var result = await service.DeleteAsync(999);

        // Assert
        Assert.False(result);
        repo.Verify(r => r.GetByIdAsync(999), Times.Once);
        repo.Verify(r => r.DeleteAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenProductExists_DeletesAndReturnsTrue()
    {
        // Arrange
        var repo = new Mock<IProductRepository>();
        var product = new Product { Id = 1, Name = "Milk", Price = 10m };

        repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
        repo.Setup(r => r.DeleteAsync(product)).Returns(Task.CompletedTask);

        var service = new ProductService(repo.Object);

        // Act
        var result = await service.DeleteAsync(1);

        // Assert
        Assert.True(result);
        repo.Verify(r => r.GetByIdAsync(1), Times.Once);
        repo.Verify(r => r.DeleteAsync(product), Times.Once);
    }
}
