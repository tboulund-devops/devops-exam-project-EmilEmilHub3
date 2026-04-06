using Moq;
using SimpleShop.Api.Models;
using SimpleShop.Api.Repositories;
using SimpleShop.Api.Services;

namespace SimpleShop.Tests;

public class AuthServiceTests
{
    [Fact]
    public async Task RegisterAsync_WhenUsernameMissing_ThrowsArgumentException()
    {
        var repo = new Mock<IUserRepository>();
        var service = new AuthService(repo.Object);

        var dto = new RegisterUserDto
        {
            Username = "   ",
            Email = "test@test.com",
            Password = "123456"
        };

        var act = async () => await service.RegisterAsync(dto);

        await Assert.ThrowsAsync<ArgumentException>(act);
        repo.Verify(r => r.GetByEmailAsync(It.IsAny<string>()), Times.Never);
        repo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailMissing_ThrowsArgumentException()
    {
        var repo = new Mock<IUserRepository>();
        var service = new AuthService(repo.Object);

        var dto = new RegisterUserDto
        {
            Username = "Emil",
            Email = "   ",
            Password = "123456"
        };

        var act = async () => await service.RegisterAsync(dto);

        await Assert.ThrowsAsync<ArgumentException>(act);
        repo.Verify(r => r.GetByEmailAsync(It.IsAny<string>()), Times.Never);
        repo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WhenPasswordMissing_ThrowsArgumentException()
    {
        var repo = new Mock<IUserRepository>();
        var service = new AuthService(repo.Object);

        var dto = new RegisterUserDto
        {
            Username = "Emil",
            Email = "test@test.com",
            Password = "   "
        };

        var act = async () => await service.RegisterAsync(dto);

        await Assert.ThrowsAsync<ArgumentException>(act);
        repo.Verify(r => r.GetByEmailAsync(It.IsAny<string>()), Times.Never);
        repo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailAlreadyExists_ThrowsArgumentException()
    {
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByEmailAsync("test@test.com"))
            .ReturnsAsync(new User { Id = 1, Email = "test@test.com" });

        var service = new AuthService(repo.Object);

        var dto = new RegisterUserDto
        {
            Username = "Emil",
            Email = " Test@Test.com ",
            Password = "123456"
        };

        var act = async () => await service.RegisterAsync(dto);

        await Assert.ThrowsAsync<ArgumentException>(act);
        repo.Verify(r => r.GetByEmailAsync("test@test.com"), Times.Once);
        repo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WhenValid_NormalizesAndHashesAndCallsRepository()
    {
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByEmailAsync("test@test.com"))
            .ReturnsAsync((User?)null);

        User? captured = null;

        repo.Setup(r => r.AddAsync(It.IsAny<User>()))
            .Callback<User>(u => captured = u)
            .ReturnsAsync((User u) =>
            {
                u.Id = 5;
                return u;
            });

        var service = new AuthService(repo.Object);

        var dto = new RegisterUserDto
        {
            Username = "  Emil  ",
            Email = " Test@Test.com ",
            Password = "123456"
        };

        var result = await service.RegisterAsync(dto);

        Assert.Equal(5, result.Id);
        Assert.Equal("Emil", result.Username);
        Assert.Equal("test@test.com", result.Email);
        Assert.False(string.IsNullOrWhiteSpace(result.PasswordHash));
        Assert.NotEqual("123456", result.PasswordHash);

        Assert.NotNull(captured);
        Assert.Equal("Emil", captured!.Username);
        Assert.Equal("test@test.com", captured.Email);
        Assert.False(string.IsNullOrWhiteSpace(captured.PasswordHash));

        repo.Verify(r => r.GetByEmailAsync("test@test.com"), Times.Once);
        repo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WhenEmailMissing_ThrowsArgumentException()
    {
        var repo = new Mock<IUserRepository>();
        var service = new AuthService(repo.Object);

        var dto = new LoginDto
        {
            Email = "   ",
            Password = "123456"
        };

        var act = async () => await service.LoginAsync(dto);

        await Assert.ThrowsAsync<ArgumentException>(act);
        repo.Verify(r => r.GetByEmailAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordMissing_ThrowsArgumentException()
    {
        var repo = new Mock<IUserRepository>();
        var service = new AuthService(repo.Object);

        var dto = new LoginDto
        {
            Email = "test@test.com",
            Password = "   "
        };

        var act = async () => await service.LoginAsync(dto);

        await Assert.ThrowsAsync<ArgumentException>(act);
        repo.Verify(r => r.GetByEmailAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByEmailAsync("test@test.com"))
            .ReturnsAsync((User?)null);

        var service = new AuthService(repo.Object);

        var dto = new LoginDto
        {
            Email = " Test@Test.com ",
            Password = "123456"
        };

        var result = await service.LoginAsync(dto);

        Assert.Null(result);
        repo.Verify(r => r.GetByEmailAsync("test@test.com"), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordIsWrong_ReturnsNull()
    {
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByEmailAsync("test@test.com"))
            .ReturnsAsync(new User
            {
                Id = 1,
                Username = "Emil",
                Email = "test@test.com",
                PasswordHash = "NOT_THE_RIGHT_HASH"
            });

        var service = new AuthService(repo.Object);

        var dto = new LoginDto
        {
            Email = "test@test.com",
            Password = "123456"
        };

        var result = await service.LoginAsync(dto);

        Assert.Null(result);
        repo.Verify(r => r.GetByEmailAsync("test@test.com"), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordIsCorrect_ReturnsUser()
    {
        var registerRepo = new Mock<IUserRepository>();
        registerRepo.Setup(r => r.GetByEmailAsync("test@test.com"))
            .ReturnsAsync((User?)null);

        User? createdUser = null;
        registerRepo.Setup(r => r.AddAsync(It.IsAny<User>()))
            .Callback<User>(u => createdUser = u)
            .ReturnsAsync((User u) =>
            {
                u.Id = 10;
                return u;
            });

        var registerService = new AuthService(registerRepo.Object);

        await registerService.RegisterAsync(new RegisterUserDto
        {
            Username = "Emil",
            Email = "test@test.com",
            Password = "123456"
        });

        Assert.NotNull(createdUser);

        var loginRepo = new Mock<IUserRepository>();
        loginRepo.Setup(r => r.GetByEmailAsync("test@test.com"))
            .ReturnsAsync(createdUser);

        var loginService = new AuthService(loginRepo.Object);

        var result = await loginService.LoginAsync(new LoginDto
        {
            Email = "test@test.com",
            Password = "123456"
        });

        Assert.NotNull(result);
        Assert.Equal("Emil", result!.Username);
        Assert.Equal("test@test.com", result.Email);
    }
}