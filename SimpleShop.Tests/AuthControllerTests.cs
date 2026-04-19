using Microsoft.AspNetCore.Mvc;
using Moq;
using SimpleShop.Api.Controllers;
using SimpleShop.Api.Models;
using SimpleShop.Api.Repositories;
using SimpleShop.Api.Services;

namespace SimpleShop.Tests;

/// <summary>
/// Unit tests for <see cref="AuthController"/>.
/// 
/// Test structure follows the AAA pattern:
/// Arrange  - Prepare mocks, services, controller, and input data.
/// Act      - Execute the controller action.
/// Assert   - Verify the returned HTTP response and expected behavior.
/// </summary>
public class AuthControllerTests
{
    [Fact]
    public async Task Register_WhenValid_ReturnsCreated()
    {
        // Arrange
        var repo = new Mock<IUserRepository>();

        repo.Setup(r => r.GetByEmailAsync("test@test.com"))
            .ReturnsAsync((User?)null);

        repo.Setup(r => r.AddAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) =>
            {
                u.Id = 1;
                return u;
            });

        var service = new AuthService(repo.Object);
        var controller = new AuthController(service);

        var dto = new RegisterUserDto
        {
            Username = "Emil",
            Email = "test@test.com",
            Password = "123456"
        };

        // Act
        var result = await controller.Register(dto);

        // Assert
        var created = Assert.IsType<CreatedResult>(result.Result);
        Assert.Equal("/api/auth/1", created.Location);
    }

    [Fact]
    public async Task Register_WhenInvalid_ReturnsBadRequest()
    {
        // Arrange
        var repo = new Mock<IUserRepository>();
        var service = new AuthService(repo.Object);
        var controller = new AuthController(service);

        var dto = new RegisterUserDto
        {
            Username = "",
            Email = "test@test.com",
            Password = "123456"
        };

        // Act
        var result = await controller.Register(dto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Login_WhenCredentialsAreValid_ReturnsOk()
    {
        // Arrange
        var registerRepo = new Mock<IUserRepository>();

        registerRepo.Setup(r => r.GetByEmailAsync("test@test.com"))
            .ReturnsAsync((User?)null);

        User? createdUser = null;

        registerRepo.Setup(r => r.AddAsync(It.IsAny<User>()))
            .Callback<User>(u => createdUser = u)
            .ReturnsAsync((User u) =>
            {
                u.Id = 1;
                return u;
            });

        var registerService = new AuthService(registerRepo.Object);

        await registerService.RegisterAsync(new RegisterUserDto
        {
            Username = "Emil",
            Email = "test@test.com",
            Password = "123456"
        });

        var loginRepo = new Mock<IUserRepository>();

        loginRepo.Setup(r => r.GetByEmailAsync("test@test.com"))
            .ReturnsAsync(createdUser);

        var controller = new AuthController(new AuthService(loginRepo.Object));

        var dto = new LoginDto
        {
            Email = "test@test.com",
            Password = "123456"
        };

        // Act
        var result = await controller.Login(dto);

        // Assert
        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task Login_WhenCredentialsAreWrong_ReturnsUnauthorized()
    {
        // Arrange
        var repo = new Mock<IUserRepository>();

        repo.Setup(r => r.GetByEmailAsync("test@test.com"))
            .ReturnsAsync(new User
            {
                Id = 1,
                Username = "Emil",
                Email = "test@test.com",
                PasswordHash = "WRONG_HASH"
            });

        var controller = new AuthController(new AuthService(repo.Object));

        var dto = new LoginDto
        {
            Email = "test@test.com",
            Password = "123456"
        };

        // Act
        var result = await controller.Login(dto);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result.Result);
    }

    [Fact]
    public async Task Login_WhenInputInvalid_ReturnsBadRequest()
    {
        // Arrange
        var repo = new Mock<IUserRepository>();
        var controller = new AuthController(new AuthService(repo.Object));

        var dto = new LoginDto
        {
            Email = "",
            Password = "123456"
        };

        // Act
        var result = await controller.Login(dto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
}