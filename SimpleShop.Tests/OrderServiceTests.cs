using Moq;
using SimpleShop.Api.Models;
using SimpleShop.Api.Repositories;
using SimpleShop.Api.Services;

namespace SimpleShop.Tests;

public class OrderServiceTests
{
    [Fact]
    public async Task CreateOrderAsync_WhenUserIdInvalid_ThrowsArgumentException()
    {
        var orderRepo = new Mock<IOrderRepository>();
        var cartRepo = new Mock<ICartRepository>();
        var userRepo = new Mock<IUserRepository>();

        var service = new OrderService(orderRepo.Object, cartRepo.Object, userRepo.Object);

        var dto = new CreateOrderDto
        {
            UserId = 0
        };

        var act = async () => await service.CreateOrderAsync(dto);

        await Assert.ThrowsAsync<ArgumentException>(act);
        orderRepo.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public async Task CreateOrderAsync_WhenUserNotFound_ThrowsArgumentException()
    {
        var orderRepo = new Mock<IOrderRepository>();
        var cartRepo = new Mock<ICartRepository>();
        var userRepo = new Mock<IUserRepository>();

        userRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((User?)null);

        var service = new OrderService(orderRepo.Object, cartRepo.Object, userRepo.Object);

        var dto = new CreateOrderDto
        {
            UserId = 1
        };

        var act = async () => await service.CreateOrderAsync(dto);

        await Assert.ThrowsAsync<ArgumentException>(act);
        orderRepo.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public async Task CreateOrderAsync_WhenCartIsEmpty_ThrowsArgumentException()
    {
        var orderRepo = new Mock<IOrderRepository>();
        var cartRepo = new Mock<ICartRepository>();
        var userRepo = new Mock<IUserRepository>();

        userRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new User
        {
            Id = 1,
            Username = "Emil",
            Email = "test@test.com",
            PasswordHash = "HASH"
        });

        cartRepo.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync(new List<CartItem>());

        var service = new OrderService(orderRepo.Object, cartRepo.Object, userRepo.Object);

        var dto = new CreateOrderDto
        {
            UserId = 1
        };

        var act = async () => await service.CreateOrderAsync(dto);

        await Assert.ThrowsAsync<ArgumentException>(act);
        orderRepo.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public async Task CreateOrderAsync_WhenValid_ReturnsCreatedOrderResponse()
    {
        var orderRepo = new Mock<IOrderRepository>();
        var cartRepo = new Mock<ICartRepository>();
        var userRepo = new Mock<IUserRepository>();

        userRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new User
        {
            Id = 1,
            Username = "Emil",
            Email = "test@test.com",
            PasswordHash = "HASH"
        });

        cartRepo.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync(new List<CartItem>
        {
            new()
            {
                Id = 1,
                UserId = 1,
                ProductId = 10,
                Quantity = 2,
                Product = new Product
                {
                    Id = 10,
                    Name = "Milk",
                    Price = 10m
                }
            },
            new()
            {
                Id = 2,
                UserId = 1,
                ProductId = 20,
                Quantity = 1,
                Product = new Product
                {
                    Id = 20,
                    Name = "Bread",
                    Price = 25m
                }
            }
        });

        orderRepo.Setup(r => r.AddAsync(It.IsAny<Order>()))
            .ReturnsAsync((Order order) =>
            {
                order.Id = 99;

                if (order.OrderLines.Count > 0)
                {
                    order.OrderLines[0].Id = 100;
                    order.OrderLines[0].Product = new Product
                    {
                        Id = 10,
                        Name = "Milk",
                        Price = 10m
                    };
                }

                if (order.OrderLines.Count > 1)
                {
                    order.OrderLines[1].Id = 101;
                    order.OrderLines[1].Product = new Product
                    {
                        Id = 20,
                        Name = "Bread",
                        Price = 25m
                    };
                }

                return order;
            });

        var service = new OrderService(orderRepo.Object, cartRepo.Object, userRepo.Object);

        var dto = new CreateOrderDto
        {
            UserId = 1
        };

        var result = await service.CreateOrderAsync(dto);

        Assert.Equal(99, result.Id);
        Assert.Equal(1, result.UserId);
        Assert.Equal(2, result.OrderLines.Count);

        Assert.Equal(10, result.OrderLines[0].ProductId);
        Assert.Equal("Milk", result.OrderLines[0].ProductName);
        Assert.Equal(2, result.OrderLines[0].Quantity);
        Assert.Equal(10m, result.OrderLines[0].UnitPrice);
        Assert.Equal(20m, result.OrderLines[0].LineTotal);

        Assert.Equal(20, result.OrderLines[1].ProductId);
        Assert.Equal("Bread", result.OrderLines[1].ProductName);
        Assert.Equal(1, result.OrderLines[1].Quantity);
        Assert.Equal(25m, result.OrderLines[1].UnitPrice);
        Assert.Equal(25m, result.OrderLines[1].LineTotal);

        Assert.Equal(45m, result.TotalPrice);

        orderRepo.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Once);
    }
}