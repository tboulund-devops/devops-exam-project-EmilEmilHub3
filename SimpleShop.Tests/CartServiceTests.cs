using Moq;
using SimpleShop.Api.Models;
using SimpleShop.Api.Repositories;
using SimpleShop.Api.Services;

namespace SimpleShop.Tests;

public class CartServiceTests
{
    [Fact]
    public async Task AddItemAsync_WhenUserIdInvalid_ThrowsArgumentException()
    {
        var cartRepo = new Mock<ICartRepository>();
        var userRepo = new Mock<IUserRepository>();
        var productRepo = new Mock<IProductRepository>();

        var service = new CartService(cartRepo.Object, userRepo.Object, productRepo.Object);

        var dto = new AddCartItemDto
        {
            UserId = 0,
            ProductId = 1,
            Quantity = 1
        };

        var act = async () => await service.AddItemAsync(dto);

        await Assert.ThrowsAsync<ArgumentException>(act);
        cartRepo.Verify(r => r.AddAsync(It.IsAny<CartItem>()), Times.Never);
    }

    [Fact]
    public async Task AddItemAsync_WhenProductIdInvalid_ThrowsArgumentException()
    {
        var cartRepo = new Mock<ICartRepository>();
        var userRepo = new Mock<IUserRepository>();
        var productRepo = new Mock<IProductRepository>();

        var service = new CartService(cartRepo.Object, userRepo.Object, productRepo.Object);

        var dto = new AddCartItemDto
        {
            UserId = 1,
            ProductId = 0,
            Quantity = 1
        };

        var act = async () => await service.AddItemAsync(dto);

        await Assert.ThrowsAsync<ArgumentException>(act);
        cartRepo.Verify(r => r.AddAsync(It.IsAny<CartItem>()), Times.Never);
    }

    [Fact]
    public async Task AddItemAsync_WhenQuantityInvalid_ThrowsArgumentException()
    {
        var cartRepo = new Mock<ICartRepository>();
        var userRepo = new Mock<IUserRepository>();
        var productRepo = new Mock<IProductRepository>();

        var service = new CartService(cartRepo.Object, userRepo.Object, productRepo.Object);

        var dto = new AddCartItemDto
        {
            UserId = 1,
            ProductId = 1,
            Quantity = 0
        };

        var act = async () => await service.AddItemAsync(dto);

        await Assert.ThrowsAsync<ArgumentException>(act);
        cartRepo.Verify(r => r.AddAsync(It.IsAny<CartItem>()), Times.Never);
    }

    [Fact]
    public async Task AddItemAsync_WhenUserNotFound_ThrowsArgumentException()
    {
        var cartRepo = new Mock<ICartRepository>();
        var userRepo = new Mock<IUserRepository>();
        var productRepo = new Mock<IProductRepository>();

        userRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((User?)null);

        var service = new CartService(cartRepo.Object, userRepo.Object, productRepo.Object);

        var dto = new AddCartItemDto
        {
            UserId = 1,
            ProductId = 2,
            Quantity = 3
        };

        var act = async () => await service.AddItemAsync(dto);

        await Assert.ThrowsAsync<ArgumentException>(act);
        cartRepo.Verify(r => r.AddAsync(It.IsAny<CartItem>()), Times.Never);
    }

    [Fact]
    public async Task AddItemAsync_WhenProductNotFound_ThrowsArgumentException()
    {
        var cartRepo = new Mock<ICartRepository>();
        var userRepo = new Mock<IUserRepository>();
        var productRepo = new Mock<IProductRepository>();

        userRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new User { Id = 1, Username = "Emil" });
        productRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync((Product?)null);

        var service = new CartService(cartRepo.Object, userRepo.Object, productRepo.Object);

        var dto = new AddCartItemDto
        {
            UserId = 1,
            ProductId = 2,
            Quantity = 3
        };

        var act = async () => await service.AddItemAsync(dto);

        await Assert.ThrowsAsync<ArgumentException>(act);
        cartRepo.Verify(r => r.AddAsync(It.IsAny<CartItem>()), Times.Never);
    }

    [Fact]
    public async Task AddItemAsync_WhenValid_ReturnsCreatedItemResponse()
    {
        var cartRepo = new Mock<ICartRepository>();
        var userRepo = new Mock<IUserRepository>();
        var productRepo = new Mock<IProductRepository>();

        userRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new User { Id = 1, Username = "Emil" });

        var product = new Product { Id = 2, Name = "Milk", Price = 15m };
        productRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(product);

        cartRepo.Setup(r => r.AddAsync(It.IsAny<CartItem>()))
            .ReturnsAsync((CartItem item) => new CartItem
            {
                Id = 10,
                UserId = item.UserId,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Product = product
            });

        var service = new CartService(cartRepo.Object, userRepo.Object, productRepo.Object);

        var dto = new AddCartItemDto
        {
            UserId = 1,
            ProductId = 2,
            Quantity = 3
        };

        var result = await service.AddItemAsync(dto);

        Assert.Equal(10, result.Id);
        Assert.Equal(2, result.ProductId);
        Assert.Equal("Milk", result.ProductName);
        Assert.Equal(15m, result.Price);
        Assert.Equal(3, result.Quantity);

        cartRepo.Verify(r => r.AddAsync(It.IsAny<CartItem>()), Times.Once);
    }

    [Fact]
    public async Task GetCartAsync_WhenUserIdInvalid_ThrowsArgumentException()
    {
        var cartRepo = new Mock<ICartRepository>();
        var userRepo = new Mock<IUserRepository>();
        var productRepo = new Mock<IProductRepository>();

        var service = new CartService(cartRepo.Object, userRepo.Object, productRepo.Object);

        var act = async () => await service.GetCartAsync(0);

        await Assert.ThrowsAsync<ArgumentException>(act);
        cartRepo.Verify(r => r.GetByUserIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetCartAsync_WhenUserNotFound_ThrowsArgumentException()
    {
        var cartRepo = new Mock<ICartRepository>();
        var userRepo = new Mock<IUserRepository>();
        var productRepo = new Mock<IProductRepository>();

        userRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((User?)null);

        var service = new CartService(cartRepo.Object, userRepo.Object, productRepo.Object);

        var act = async () => await service.GetCartAsync(1);

        await Assert.ThrowsAsync<ArgumentException>(act);
        cartRepo.Verify(r => r.GetByUserIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetCartAsync_WhenCartIsEmpty_ReturnsEmptyCartWithZeroTotalPrice()
    {
        var cartRepo = new Mock<ICartRepository>();
        var userRepo = new Mock<IUserRepository>();
        var productRepo = new Mock<IProductRepository>();

        userRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new User { Id = 1, Username = "Emil" });
        cartRepo.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync(new List<CartItem>());

        var service = new CartService(cartRepo.Object, userRepo.Object, productRepo.Object);

        var result = await service.GetCartAsync(1);

        Assert.Equal(1, result.UserId);
        Assert.Empty(result.Items);
        Assert.Equal(0m, result.TotalPrice);
    }

    [Fact]
    public async Task GetCartAsync_WhenCartHasItems_ReturnsItemsAndTotalPrice()
    {
        var cartRepo = new Mock<ICartRepository>();
        var userRepo = new Mock<IUserRepository>();
        var productRepo = new Mock<IProductRepository>();

        userRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new User { Id = 1, Username = "Emil" });

        cartRepo.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync(new List<CartItem>
        {
            new()
            {
                Id = 1,
                UserId = 1,
                ProductId = 10,
                Quantity = 2,
                Product = new Product { Id = 10, Name = "Milk", Price = 10m }
            },
            new()
            {
                Id = 2,
                UserId = 1,
                ProductId = 20,
                Quantity = 1,
                Product = new Product { Id = 20, Name = "Bread", Price = 25m }
            }
        });

        var service = new CartService(cartRepo.Object, userRepo.Object, productRepo.Object);

        var result = await service.GetCartAsync(1);

        Assert.Equal(1, result.UserId);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(45m, result.TotalPrice);
        Assert.Equal("Milk", result.Items[0].ProductName);
        Assert.Equal("Bread", result.Items[1].ProductName);
    }
}