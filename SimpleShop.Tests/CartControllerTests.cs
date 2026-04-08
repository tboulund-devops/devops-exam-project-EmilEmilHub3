using Microsoft.AspNetCore.Mvc;
using Moq;
using SimpleShop.Api.Controllers;
using SimpleShop.Api.Models;
using SimpleShop.Api.Repositories;
using SimpleShop.Api.Services;

namespace SimpleShop.Tests;

public class CartControllerTests
{
    [Fact]
    public async Task AddItem_WhenValid_ReturnsCreated()
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
                Id = 99,
                UserId = item.UserId,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Product = product
            });

        var controller = new CartController(new CartService(cartRepo.Object, userRepo.Object, productRepo.Object));

        var dto = new AddCartItemDto
        {
            UserId = 1,
            ProductId = 2,
            Quantity = 3
        };

        var result = await controller.AddItem(dto);

        var created = Assert.IsType<CreatedResult>(result.Result);
        Assert.Equal("/api/cart/items/99", created.Location);

        var body = Assert.IsType<CartItemResponseDto>(created.Value);
        Assert.Equal("Milk", body.ProductName);
        Assert.Equal(15m, body.Price);
    }

    [Fact]
    public async Task AddItem_WhenInvalid_ReturnsBadRequest()
    {
        var cartRepo = new Mock<ICartRepository>();
        var userRepo = new Mock<IUserRepository>();
        var productRepo = new Mock<IProductRepository>();

        var controller = new CartController(new CartService(cartRepo.Object, userRepo.Object, productRepo.Object));

        var result = await controller.AddItem(new AddCartItemDto
        {
            UserId = 0,
            ProductId = 2,
            Quantity = 1
        });

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetCart_WhenValid_ReturnsOk()
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
                ProductId = 2,
                Quantity = 2,
                Product = new Product { Id = 2, Name = "Milk", Price = 10m }
            }
        });

        var controller = new CartController(new CartService(cartRepo.Object, userRepo.Object, productRepo.Object));

        var result = await controller.GetCart(1);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var body = Assert.IsType<CartResponseDto>(ok.Value);

        Assert.Equal(1, body.UserId);
        Assert.Single(body.Items);
        Assert.Equal(20m, body.TotalPrice);
    }

    [Fact]
    public async Task GetCart_WhenInvalid_ReturnsBadRequest()
    {
        var cartRepo = new Mock<ICartRepository>();
        var userRepo = new Mock<IUserRepository>();
        var productRepo = new Mock<IProductRepository>();

        var controller = new CartController(new CartService(cartRepo.Object, userRepo.Object, productRepo.Object));

        var result = await controller.GetCart(0);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateItemQuantity_WhenValid_ReturnsOk()
    {
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

        var controller = new CartController(new CartService(cartRepo.Object, userRepo.Object, productRepo.Object));

        var dto = new UpdateCartItemQuantityDto
        {
            Quantity = 4
        };

        var result = await controller.UpdateItemQuantity(1, dto);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var body = Assert.IsType<CartItemResponseDto>(ok.Value);

        Assert.Equal(1, body.Id);
        Assert.Equal(4, body.Quantity);
        Assert.Equal("Milk", body.ProductName);
    }

    [Fact]
    public async Task UpdateItemQuantity_WhenItemNotFound_ReturnsNotFound()
    {
        var cartRepo = new Mock<ICartRepository>();
        var userRepo = new Mock<IUserRepository>();
        var productRepo = new Mock<IProductRepository>();

        cartRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((CartItem?)null);

        var controller = new CartController(new CartService(cartRepo.Object, userRepo.Object, productRepo.Object));

        var dto = new UpdateCartItemQuantityDto
        {
            Quantity = 4
        };

        var result = await controller.UpdateItemQuantity(1, dto);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateItemQuantity_WhenInvalid_ReturnsBadRequest()
    {
        var cartRepo = new Mock<ICartRepository>();
        var userRepo = new Mock<IUserRepository>();
        var productRepo = new Mock<IProductRepository>();

        var controller = new CartController(new CartService(cartRepo.Object, userRepo.Object, productRepo.Object));

        var dto = new UpdateCartItemQuantityDto
        {
            Quantity = 0
        };

        var result = await controller.UpdateItemQuantity(1, dto);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task RemoveItem_WhenValid_ReturnsNoContent()
    {
        var cartRepo = new Mock<ICartRepository>();
        var userRepo = new Mock<IUserRepository>();
        var productRepo = new Mock<IProductRepository>();

        cartRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new CartItem
        {
            Id = 1,
            UserId = 1,
            ProductId = 2,
            Quantity = 3
        });

        var controller = new CartController(new CartService(cartRepo.Object, userRepo.Object, productRepo.Object));

        var result = await controller.RemoveItem(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task RemoveItem_WhenItemNotFound_ReturnsNotFound()
    {
        var cartRepo = new Mock<ICartRepository>();
        var userRepo = new Mock<IUserRepository>();
        var productRepo = new Mock<IProductRepository>();

        cartRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((CartItem?)null);

        var controller = new CartController(new CartService(cartRepo.Object, userRepo.Object, productRepo.Object));

        var result = await controller.RemoveItem(1);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task RemoveItem_WhenInvalid_ReturnsBadRequest()
    {
        var cartRepo = new Mock<ICartRepository>();
        var userRepo = new Mock<IUserRepository>();
        var productRepo = new Mock<IProductRepository>();

        var controller = new CartController(new CartService(cartRepo.Object, userRepo.Object, productRepo.Object));

        var result = await controller.RemoveItem(0);

        Assert.IsType<BadRequestObjectResult>(result);
    }
}