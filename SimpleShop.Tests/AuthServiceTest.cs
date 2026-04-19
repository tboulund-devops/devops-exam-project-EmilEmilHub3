using Moq;
using SimpleShop.Api.Models;
using SimpleShop.Api.Repositories;
using SimpleShop.Api.Services;

namespace SimpleShop.Tests;

/// <summary>
/// Unit tests for <see cref="AuthService"/>.
/// Tests follow the Arrange, Act, Assert (AAA) pattern
/// to keep test intent readable and maintainable.
/// </summary>
public class AuthServiceTests
{
    [Fact]
    public async Task RegisterAsync_WhenUsernameMissing_ThrowsArgumentException()
    {
        // Arrange
        var repo = new Mock<IUserRepository>();
        var service = new AuthService(repo.Object);

        var dto = new RegisterUserDto
        {
            Username = "   ",
            Email = "test@test.com",
            Password = "123456"
        };

        // Act
        var act = async () => await service.RegisterAsync(dto);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
        repo.Verify(r => r.GetByEmailAsync(It.IsAny<string>()), Times.Never);
        repo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailMissing_ThrowsArgumentException()
    {
        // Arrange
        var repo = new Mock<IUserRepository>();
        var service = new AuthService(repo.Object);

        var dto = new RegisterUserDto
        {
            Username = "Emil",
            Email = "   ",
            Password = "123456"
        };

        // Act
        var act = async () => await service.RegisterAsync(dto);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
        repo.Verify(r => r.GetByEmailAsync(It.IsAny<string>()), Times.Never);
        repo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WhenPasswordMissing_ThrowsArgumentException()
    {
        // Arrange
        var repo = new Mock<IUserRepository>();
        var service = new AuthService(repo.Object);

        var dto = new RegisterUserDto
        {
            Username = "Emil",
            Email = "test@test.com",
            Password = "   "
        };

        // Act
        var act = async () => await service.RegisterAsync(dto);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
        repo.Verify(r => r.GetByEmailAsync(It.IsAny<string>()), Times.Never);
        repo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailAlreadyExists_ThrowsArgumentException()
    {
        // Arrange
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

        // Act
        var act = async () => await service.RegisterAsync(dto);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
        repo.Verify(r => r.GetByEmailAsync("test@test.com"), Times.Once);
        repo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WhenValid_NormalizesAndHashesAndCallsRepository()
    {
        // Arrange
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

        // Act
        var result = await service.RegisterAsync(dto);

        // Assert
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
        // Arrange
        var repo = new Mock<IUserRepository>();
        var service = new AuthService(repo.Object);

        var dto = new LoginDto
        {
            Email = "   ",
            Password = "123456"
        };

        // Act
        var act = async () => await service.LoginAsync(dto);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
        repo.Verify(r => r.GetByEmailAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordMissing_ThrowsArgumentException()
    {
        // Arrange
        var repo = new Mock<IUserRepository>();
        var service = new AuthService(repo.Object);

        var dto = new LoginDto
        {
            Email = "test@test.com",
            Password = "   "
        };

        // Act
        var act = async () => await service.LoginAsync(dto);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
        repo.Verify(r => r.GetByEmailAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        // Arrange
        var repo = new Mock<IUserRepository>();

        repo.Setup(r => r.GetByEmailAsync("test@test.com"))
            .ReturnsAsync((User?)null);

        var service = new AuthService(repo.Object);

        var dto = new LoginDto
        {
            Email = " Test@Test.com ",
            Password = "123456"
        };

        // Act
        var result = await service.LoginAsync(dto);

        // Assert
        Assert.Null(result);
        repo.Verify(r => r.GetByEmailAsync("test@test.com"), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordIsWrong_ReturnsNull()
    {
        // Arrange
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

        // Act
        var result = await service.LoginAsync(dto);

        // Assert
        Assert.Null(result);
        repo.Verify(r => r.GetByEmailAsync("test@test.com"), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordIsCorrect_ReturnsUser()
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

        var dto = new LoginDto
        {
            Email = "test@test.com",
            Password = "123456"
        };

        // Act
        var result = await loginService.LoginAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Emil", result!.Username);
        Assert.Equal("test@test.com", result.Email);
    }
}