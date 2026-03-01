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
    public async Task GetAll_ReturnsOk_WithProducts()
    {
        // Arrange
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Product>
        {
            new() { Id = 1, Name = "Milk", Price = 10m }
        });

        var service = new ProductService(repo.Object);
        var controller = new ProductsController(service);

        // Act
        var result = await controller.GetAll();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var products = Assert.IsType<List<Product>>(ok.Value);

        Assert.Single(products);
        Assert.Equal("Milk", products[0].Name);
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
}