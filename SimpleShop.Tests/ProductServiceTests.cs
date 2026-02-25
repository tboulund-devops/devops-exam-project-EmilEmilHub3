using Moq;
using SimpleShop.Api.Models;
using SimpleShop.Api.Repositories;
using SimpleShop.Api.Services;

namespace SimpleShop.Tests;

public class ProductServiceTests
{
    [Fact]
    public async Task CreateAsync_ThrowsIfNameMissing()
    {
        var repo = new Mock<IProductRepository>();
        var service = new ProductService(repo.Object);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateAsync(new CreateProductDto { Name = "   ", Price = 10m }));
    }

    [Fact]
    public async Task CreateAsync_CallsRepository()
    {
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.AddAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) => { p.Id = 1; return p; });

        var service = new ProductService(repo.Object);

        var created = await service.CreateAsync(new CreateProductDto { Name = "Milk", Price = 12.5m });

        Assert.Equal(1, created.Id);
        repo.Verify(r => r.AddAsync(It.Is<Product>(p => p.Name == "Milk" && p.Price == 12.5m)), Times.Once);
    }
}