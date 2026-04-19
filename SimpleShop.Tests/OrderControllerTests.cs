using Microsoft.AspNetCore.Mvc;
using Moq;
using SimpleShop.Api.Controllers;
using SimpleShop.Api.Models;
using SimpleShop.Api.Repositories;
using SimpleShop.Api.Services;

namespace SimpleShop.Tests;

/// <summary>
/// Unit tests for <see cref="OrdersController"/>.
/// Tests verify controller response behavior for order creation.
/// Tests follow the Arrange, Act, Assert (AAA) pattern.
/// </summary>
public class OrdersControllerTests
{
    [Fact]
    public async Task Create_WhenValid_ReturnsCreated()
    {
        // Arrange
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
            }
        });

        orderRepo.Setup(r => r.AddAsync(It.IsAny<Order>()))
            .ReturnsAsync((Order order) =>
            {
                order.Id = 50;
                order.OrderLines[0].Id = 60;
                order.OrderLines[0].Product = new Product
                {
                    Id = 10,
                    Name = "Milk",
                    Price = 10m
                };
                return order;
            });

        var controller = new OrdersController(
            new OrderService(orderRepo.Object, cartRepo.Object, userRepo.Object));

        var dto = new CreateOrderDto
        {
            UserId = 1
        };

        // Act
        var result = await controller.Create(dto);

        // Assert
        var created = Assert.IsType<CreatedResult>(result.Result);
        Assert.Equal("/api/orders/50", created.Location);

        var body = Assert.IsType<OrderResponseDto>(created.Value);
        Assert.Equal(50, body.Id);
        Assert.Equal(1, body.UserId);
        Assert.Single(body.OrderLines);
        Assert.Equal(20m, body.TotalPrice);
    }

    [Fact]
    public async Task Create_WhenInvalid_ReturnsBadRequest()
    {
        // Arrange
        var orderRepo = new Mock<IOrderRepository>();
        var cartRepo = new Mock<ICartRepository>();
        var userRepo = new Mock<IUserRepository>();

        var controller = new OrdersController(
            new OrderService(orderRepo.Object, cartRepo.Object, userRepo.Object));

        var dto = new CreateOrderDto
        {
            UserId = 0
        };

        // Act
        var result = await controller.Create(dto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
}