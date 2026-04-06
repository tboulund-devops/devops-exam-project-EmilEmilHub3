using Microsoft.AspNetCore.Mvc;
using Moq;
using SimpleShop.Api.Controllers;
using SimpleShop.Api.Models;
using SimpleShop.Api.Repositories;
using SimpleShop.Api.Services;

namespace SimpleShop.Tests;

public class AuthControllerTests
{
    [Fact]
    public async Task Register_WhenValid_ReturnsCreated()
    {
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

        var result = await controller.Register(dto);

        var created = Assert.IsType<CreatedResult>(result.Result);
        Assert.Equal("/api/auth/1", created.Location);
    }

    [Fact]
    public async Task Register_WhenInvalid_ReturnsBadRequest()
    {
        var repo = new Mock<IUserRepository>();
        var service = new AuthService(repo.Object);
        var controller = new AuthController(service);

        var dto = new RegisterUserDto
        {
            Username = "",
            Email = "test@test.com",
            Password = "123456"
        };

        var result = await controller.Register(dto);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Login_WhenCredentialsAreValid_ReturnsOk()
    {
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

        var result = await controller.Login(new LoginDto
        {
            Email = "test@test.com",
            Password = "123456"
        });

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task Login_WhenCredentialsAreWrong_ReturnsUnauthorized()
    {
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

        var result = await controller.Login(new LoginDto
        {
            Email = "test@test.com",
            Password = "123456"
        });

        Assert.IsType<UnauthorizedObjectResult>(result.Result);
    }

    [Fact]
    public async Task Login_WhenInputInvalid_ReturnsBadRequest()
    {
        var repo = new Mock<IUserRepository>();
        var controller = new AuthController(new AuthService(repo.Object));

        var result = await controller.Login(new LoginDto
        {
            Email = "",
            Password = "123456"
        });

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
}