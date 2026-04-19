using Moq;
using SimpleShop.Api.Models;
using SimpleShop.Api.Repositories;
using SimpleShop.Api.Services;

namespace SimpleShop.Tests;

/// <summary>
/// Unit tests for <see cref="CartService"/>.
/// Tests verify validation rules, business logic,
/// and repository interaction.
/// Tests follow the Arrange, Act, Assert (AAA) pattern.
/// </summary>
public class CartServiceTests
{
    [Fact]
    public async Task AddItemAsync_WhenUserIdInvalid_ThrowsArgumentException()
    {
        // Arrange
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

        // Act
        var act = async () => await service.AddItemAsync(dto);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
        cartRepo.Verify(r => r.AddAsync(It.IsAny<CartItem>()), Times.Never);
    }

    [Fact]
    public async Task AddItemAsync_WhenProductIdInvalid_ThrowsArgumentException()
    {
        // Arrange
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

        // Act
        var act = async () => await service.AddItemAsync(dto);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
        cartRepo.Verify(r => r.AddAsync(It.IsAny<CartItem>()), Times.Never);
    }

    [Fact]
    public async Task AddItemAsync_WhenQuantityInvalid_ThrowsArgumentException()
    {
        // Arrange
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

        // Act
        var act = async () => await service.AddItemAsync(dto);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
        cartRepo.Verify(r => r.AddAsync(It.IsAny<CartItem>()), Times.Never);
    }

    [Fact]
    public async Task AddItemAsync_WhenUserNotFound_ThrowsArgumentException()
    {
        // Arrange
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

        // Act
        var act = async () => await service.AddItemAsync(dto);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
        cartRepo.Verify(r => r.AddAsync(It.IsAny<CartItem>()), Times.Never);
    }

    [Fact]
    public async Task AddItemAsync_WhenProductNotFound_ThrowsArgumentException()
    {
        // Arrange
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

        // Act
        var act = async () => await service.AddItemAsync(dto);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
        cartRepo.Verify(r => r.AddAsync(It.IsAny<CartItem>()), Times.Never);
    }

    [Fact]
    public async Task AddItemAsync_WhenValid_ReturnsCreatedItemResponse()
    {
        // Arrange
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

        // Act
        var result = await service.AddItemAsync(dto);

        // Assert
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
        // Arrange
        var cartRepo = new Mock<ICartRepository>();
        var userRepo = new Mock<IUserRepository>();
        var productRepo = new Mock<IProductRepository>();

        var service = new CartService(cartRepo.Object, userRepo.Object, productRepo.Object);

        // Act
        var act = async () => await service.GetCartAsync(0);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
        cartRepo.Verify(r => r.GetByUserIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetCartAsync_WhenUserNotFound_ThrowsArgumentException()
    {
        // Arrange
        var cartRepo = new Mock<ICartRepository>();
        var userRepo = new Mock<IUserRepository>();
        var productRepo = new Mock<IProductRepository>();

        userRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((User?)null);

        var service = new CartService(cartRepo.Object, userRepo.Object, productRepo.Object);

        // Act
        var act = async () => await service.GetCartAsync(1);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
        cartRepo.Verify(r => r.GetByUserIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetCartAsync_WhenCartIsEmpty_ReturnsEmptyCartWithZeroTotalPrice()
    {
        // Arrange
        var cartRepo = new Mock<ICartRepository>();
        var userRepo = new Mock<IUserRepository>();
        var productRepo = new Mock<IProductRepository>();

        userRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new User { Id = 1, Username = "Emil" });
        cartRepo.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync(new List<CartItem>());

        var service = new CartService(cartRepo.Object, userRepo.Object, productRepo.Object);

        // Act
        var result = await service.GetCartAsync(1);

        // Assert
        Assert.Equal(1, result.UserId);
        Assert.Empty(result.Items);
        Assert.Equal(0m, result.TotalPrice);
    }

    [Fact]
    public async Task GetCartAsync_WhenCartHasItems_ReturnsItemsAndTotalPrice()
    {
        // Arrange
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

        // Act
        var result = await service.GetCartAsync(1);

        // Assert
        Assert.Equal(1, result.UserId);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(45m, result.TotalPrice);
        Assert.Equal("Milk", result.Items[0].ProductName);
        Assert.Equal("Bread", result.Items[1].ProductName);
    }

    [Fact]
    public async Task UpdateItemQuantityAsync_WhenIdInvalid_ThrowsArgumentException()
    {
        // Arrange
        var cartRepo = new Mock<ICartRepository>();
        var userRepo = new Mock<IUserRepository>();
        var productRepo = new Mock<IProductRepository>();

        var service = new CartService(cartRepo.Object, userRepo.Object, productRepo.Object);

        var dto = new UpdateCartItemQuantityDto
        {
            Quantity = 3
        };

        // Act
        var act = async () => await service.UpdateItemQuantityAsync(0, dto);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
        cartRepo.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task UpdateItemQuantityAsync_WhenQuantityInvalid_ThrowsArgumentException()
    {
        // Arrange
        var cartRepo = new Mock<ICartRepository>();
        var userRepo = new Mock<IUserRepository>();
        var productRepo = new Mock<IProductRepository>();

        var service = new CartService(cartRepo.Object, userRepo.Object, productRepo.Object);

        var dto = new UpdateCartItemQuantityDto
        {
            Quantity = 0
        };

        // Act
        var act = async () => await service.UpdateItemQuantityAsync(1, dto);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
        cartRepo.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task UpdateItemQuantityAsync_WhenCartItemNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var cartRepo = new Mock<ICartRepository>();
        var userRepo = new Mock<IUserRepository>();
        var productRepo = new Mock<IProductRepository>();

        cartRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((CartItem?)null);

        var service = new CartService(cartRepo.Object, userRepo.Object, productRepo.Object);

        var dto = new UpdateCartItemQuantityDto
        {
            Quantity = 5
        };

        // Act
        var act = async () => await service.UpdateItemQuantityAsync(1, dto);

        // Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(act);
        cartRepo.Verify(r => r.UpdateAsync(It.IsAny<CartItem>()), Times.Never);
    }

    [Fact]
    public async Task UpdateItemQuantityAsync_WhenValid_ReturnsUpdatedCartItemResponse()
    {
        // Arrange
        var cartRepo = new Mock<ICartRepository>();
        var userRepo = new Mock<IUserRepository>();
        var productRepo = new Mock<IProductRepository>();

        var existingItem = new CartItem
        {
            Id = 1,
            UserId = 1,
            ProductId = 2,
            Quantity = 1,
            Product = new Product
            {
                Id = 2,
                Name = "Milk",
                Price = 15m
            }
        };

        cartRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingItem);
        cartRepo.Setup(r => r.UpdateAsync(It.IsAny<CartItem>()))
            .ReturnsAsync((CartItem item) => item);

        var service = new CartService(cartRepo.Object, userRepo.Object, productRepo.Object);

        var dto = new UpdateCartItemQuantityDto
        {
            Quantity = 4
        };

        // Act
        var result = await service.UpdateItemQuantityAsync(1, dto);

        // Assert
        Assert.Equal(1, result.Id);
        Assert.Equal(2, result.ProductId);
        Assert.Equal("Milk", result.ProductName);
        Assert.Equal(15m, result.Price);
        Assert.Equal(4, result.Quantity);

        cartRepo.Verify(r => r.UpdateAsync(It.Is<CartItem>(c => c.Id == 1 && c.Quantity == 4)), Times.Once);
    }

    [Fact]
    public async Task RemoveItemAsync_WhenIdInvalid_ThrowsArgumentException()
    {
        // Arrange
        var cartRepo = new Mock<ICartRepository>();
        var userRepo = new Mock<IUserRepository>();
        var productRepo = new Mock<IProductRepository>();

        var service = new CartService(cartRepo.Object, userRepo.Object, productRepo.Object);

        // Act
        var act = async () => await service.RemoveItemAsync(0);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
        cartRepo.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task RemoveItemAsync_WhenCartItemNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var cartRepo = new Mock<ICartRepository>();
        var userRepo = new Mock<IUserRepository>();
        var productRepo = new Mock<IProductRepository>();

        cartRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((CartItem?)null);

        var service = new CartService(cartRepo.Object, userRepo.Object, productRepo.Object);

        // Act
        var act = async () => await service.RemoveItemAsync(1);

        // Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(act);
        cartRepo.Verify(r => r.DeleteAsync(It.IsAny<CartItem>()), Times.Never);
    }

    [Fact]
    public async Task RemoveItemAsync_WhenValid_DeletesCartItem()
    {
        // Arrange
        var cartRepo = new Mock<ICartRepository>();
        var userRepo = new Mock<IUserRepository>();
        var productRepo = new Mock<IProductRepository>();

        var existingItem = new CartItem
        {
            Id = 1,
            UserId = 1,
            ProductId = 2,
            Quantity = 3
        };

        cartRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingItem);

        var service = new CartService(cartRepo.Object, userRepo.Object, productRepo.Object);

        // Act
        await service.RemoveItemAsync(1);

        // Assert
        cartRepo.Verify(r => r.DeleteAsync(It.Is<CartItem>(c => c.Id == 1)), Times.Once);
    }
}