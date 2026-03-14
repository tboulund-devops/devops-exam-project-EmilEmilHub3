using Microsoft.AspNetCore.Mvc;
using Moq;
using SimpleShop.Api.Controllers;
using SimpleShop.Api.Models;
using SimpleShop.Api.Repositories;
using SimpleShop.Api.Services;

namespace SimpleShop.Tests;

public class ProductsControllerTests
{
    [Fact]
    public async Task GetAll_WithoutSearch_ReturnsOk_WithProducts()
    {
        // Arrange
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.GetAllAsync(null)).ReturnsAsync(new List<Product>
        {
            new() { Id = 1, Name = "Milk", Price = 10m }
        });

        var service = new ProductService(repo.Object);
        var controller = new ProductsController(service);

        // Act
        var result = await controller.GetAll(null);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var products = Assert.IsType<List<Product>>(ok.Value);

        Assert.Single(products);
        Assert.Equal("Milk", products[0].Name);
    }

    [Fact]
    public async Task GetAll_WithSearch_ReturnsFilteredProducts()
    {
        // Arrange
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.GetAllAsync("milk")).ReturnsAsync(new List<Product>
        {
            new() { Id = 1, Name = "Milk", Price = 10m }
        });

        var service = new ProductService(repo.Object);
        var controller = new ProductsController(service);

        // Act
        var result = await controller.GetAll("milk");

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var products = Assert.IsType<List<Product>>(ok.Value);

        Assert.Single(products);
        Assert.Equal("Milk", products[0].Name);
        repo.Verify(r => r.GetAllAsync("milk"), Times.Once);
    }

    [Fact]
    public async Task GetById_WhenExists_ReturnsOk_WithProduct()
    {
        // Arrange
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Product { Id = 1, Name = "Milk", Price = 10m });

        var service = new ProductService(repo.Object);
        var controller = new ProductsController(service);

        // Act
        var result = await controller.GetById(1);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var product = Assert.IsType<Product>(ok.Value);

        Assert.Equal(1, product.Id);
        Assert.Equal("Milk", product.Name);
    }

    [Fact]
    public async Task GetById_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Product?)null);

        var service = new ProductService(repo.Object);
        var controller = new ProductsController(service);

        // Act
        var result = await controller.GetById(999);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetById_WhenIdInvalid_ReturnsBadRequest()
    {
        // Arrange
        var repo = new Mock<IProductRepository>();
        var service = new ProductService(repo.Object);
        var controller = new ProductsController(service);

        // Act
        var result = await controller.GetById(0);

        // Assert
        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.NotNull(bad.Value);

        var errorProp = bad.Value!.GetType().GetProperty("error");
        Assert.NotNull(errorProp);

        var errorMessage = errorProp!.GetValue(bad.Value) as string;
        Assert.False(string.IsNullOrWhiteSpace(errorMessage));

        repo.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Create_WhenValid_ReturnsCreated_WithLocationAndBody()
    {
        // Arrange
        var repo = new Mock<IProductRepository>();

        repo.Setup(r => r.AddAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) => new Product
            {
                Id = 123,
                Name = p.Name,
                Price = p.Price
            });

        var service = new ProductService(repo.Object);
        var controller = new ProductsController(service);

        var dto = new CreateProductDto { Name = "Milk", Price = 12.5m };

        // Act
        var result = await controller.Create(dto);

        // Assert
        var created = Assert.IsType<CreatedResult>(result.Result);
        Assert.Equal("/api/products/123", created.Location);

        var body = Assert.IsType<Product>(created.Value);
        Assert.Equal(123, body.Id);
        Assert.Equal("Milk", body.Name);
        Assert.Equal(12.5m, body.Price);

        repo.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task Create_WhenNameMissing_ReturnsBadRequest()
    {
        // Arrange
        var repo = new Mock<IProductRepository>();
        var service = new ProductService(repo.Object);
        var controller = new ProductsController(service);

        var dto = new CreateProductDto { Name = "   ", Price = 12.5m };

        // Act
        var result = await controller.Create(dto);

        // Assert
        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.NotNull(bad.Value);

        var errorProp = bad.Value!.GetType().GetProperty("error");
        Assert.NotNull(errorProp);

        var errorMessage = errorProp!.GetValue(bad.Value) as string;
        Assert.False(string.IsNullOrWhiteSpace(errorMessage));

        repo.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task Update_WhenValid_ReturnsOk_WithUpdatedProduct()
    {
        // Arrange
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Product { Id = 1, Name = "Old", Price = 1m });

        repo.Setup(r => r.UpdateAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) => p);

        var service = new ProductService(repo.Object);
        var controller = new ProductsController(service);

        var dto = new CreateProductDto { Name = "  New  ", Price = 99m };

        // Act
        var result = await controller.Update(1, dto);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var product = Assert.IsType<Product>(ok.Value);

        Assert.Equal(1, product.Id);
        Assert.Equal("New", product.Name);
        Assert.Equal(99m, product.Price);

        repo.Verify(r => r.GetByIdAsync(1), Times.Once);
        repo.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task Update_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Product?)null);

        var service = new ProductService(repo.Object);
        var controller = new ProductsController(service);

        var dto = new CreateProductDto { Name = "Milk", Price = 10m };

        // Act
        var result = await controller.Update(999, dto);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
        repo.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task Update_WhenNameMissing_ReturnsBadRequest()
    {
        // Arrange
        var repo = new Mock<IProductRepository>();
        var service = new ProductService(repo.Object);
        var controller = new ProductsController(service);

        var dto = new CreateProductDto { Name = "   ", Price = 10m };

        // Act
        var result = await controller.Update(1, dto);

        // Assert
        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.NotNull(bad.Value);

        var errorProp = bad.Value!.GetType().GetProperty("error");
        Assert.NotNull(errorProp);

        var errorMessage = errorProp!.GetValue(bad.Value) as string;
        Assert.False(string.IsNullOrWhiteSpace(errorMessage));

        repo.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        repo.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task Delete_WhenProductExists_ReturnsNoContent()
    {
        // Arrange
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Product { Id = 1, Name = "Milk", Price = 10m });
        repo.Setup(r => r.DeleteAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

        var service = new ProductService(repo.Object);
        var controller = new ProductsController(service);

        // Act
        var result = await controller.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
        repo.Verify(r => r.GetByIdAsync(1), Times.Once);
        repo.Verify(r => r.DeleteAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task Delete_WhenProductDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Product?)null);

        var service = new ProductService(repo.Object);
        var controller = new ProductsController(service);

        // Act
        var result = await controller.Delete(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        repo.Verify(r => r.DeleteAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task Delete_WhenIdInvalid_ReturnsBadRequest()
    {
        // Arrange
        var repo = new Mock<IProductRepository>();
        var service = new ProductService(repo.Object);
        var controller = new ProductsController(service);

        // Act
        var result = await controller.Delete(0);

        // Assert
        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(bad.Value);

        var errorProp = bad.Value!.GetType().GetProperty("error");
        Assert.NotNull(errorProp);

        var errorMessage = errorProp!.GetValue(bad.Value) as string;
        Assert.False(string.IsNullOrWhiteSpace(errorMessage));

        repo.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        repo.Verify(r => r.DeleteAsync(It.IsAny<Product>()), Times.Never);
    }
}
