using Microsoft.Extensions.Configuration;
using Moq;
using TravelRequests.Application.Contracts.Repositories;
using TravelRequests.Application.DTOs.Auth;
using TravelRequests.Application.Services;
using TravelRequests.Domain.Entities;
using TravelRequests.Domain.Enums;
using TravelRequests.Tests.Helpers;

namespace TravelRequests.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordResetCodeRepository> _passwordResetCodeRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordResetCodeRepositoryMock = new Mock<IPasswordResetCodeRepository>();
        _configurationMock = new Mock<IConfiguration>();

        // Configurar mocks de configuración
        _configurationMock.Setup(x => x["Jwt:Key"]).Returns("TU_SUPER_SECRETO_AQUI_MUY_LARGO_Y_SEGURO_2025");
        _configurationMock.Setup(x => x["Jwt:Issuer"]).Returns("TravelRequestsApi");
        _configurationMock.Setup(x => x["Jwt:Audience"]).Returns("TravelRequestsClient");
        _configurationMock.Setup(x => x["Jwt:ExpirationMinutes"]).Returns("60");

        _authService = new AuthService(
            _userRepositoryMock.Object,
            _passwordResetCodeRepositoryMock.Object,
            _configurationMock.Object
        );
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_ShouldReturnAuthResponse()
    {
        // Arrange
        var registerRequest = TestDataHelper.CreateValidRegisterRequest();
        var expectedUser = TestDataHelper.CreateTestUser(registerRequest.Email, registerRequest.Role);

        _userRepositoryMock.Setup(x => x.ExistsByEmailAsync(registerRequest.Email))
            .ReturnsAsync(false);
        _userRepositoryMock.Setup(x => x.AddAsync(It.IsAny<User>()))
            .ReturnsAsync((User user) => user);

        // Act
        var result = await _authService.RegisterAsync(registerRequest);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Token);
        Assert.True(result.ExpiresAt > DateTime.UtcNow);
        Assert.Equal(registerRequest.Email, result.UserEmail);
        Assert.Equal(registerRequest.Name, result.UserName);
        Assert.Equal(registerRequest.Role.ToString(), result.Role);

        _userRepositoryMock.Verify(x => x.ExistsByEmailAsync(registerRequest.Email), Times.Once);
        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ShouldThrowException()
    {
        // Arrange
        var registerRequest = TestDataHelper.CreateValidRegisterRequest();

        _userRepositoryMock.Setup(x => x.ExistsByEmailAsync(registerRequest.Email))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _authService.RegisterAsync(registerRequest));

        _userRepositoryMock.Verify(x => x.ExistsByEmailAsync(registerRequest.Email), Times.Once);
        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnAuthResponse()
    {
        // Arrange
        var loginRequest = TestDataHelper.CreateValidLoginRequest();
        var user = TestDataHelper.CreateTestUser(loginRequest.Email);

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(loginRequest.Email))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(loginRequest);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Token);
        Assert.True(result.ExpiresAt > DateTime.UtcNow);
        Assert.Equal(loginRequest.Email, result.UserEmail);
        Assert.Equal(user.Name, result.UserName);
        Assert.Equal(user.Role.ToString(), result.Role);

        _userRepositoryMock.Verify(x => x.GetByEmailAsync(loginRequest.Email), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidEmail_ShouldThrowException()
    {
        // Arrange
        var loginRequest = TestDataHelper.CreateValidLoginRequest();

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(loginRequest.Email))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _authService.LoginAsync(loginRequest));

        _userRepositoryMock.Verify(x => x.GetByEmailAsync(loginRequest.Email), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ShouldThrowException()
    {
        // Arrange
        var loginRequest = TestDataHelper.CreateValidLoginRequest();
        var user = TestDataHelper.CreateTestUser(loginRequest.Email);
        // Cambiar la contraseña para que no coincida
        user.ChangePassword(BCrypt.Net.BCrypt.HashPassword("WrongPassword"));

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(loginRequest.Email))
            .ReturnsAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _authService.LoginAsync(loginRequest));

        _userRepositoryMock.Verify(x => x.GetByEmailAsync(loginRequest.Email), Times.Once);
    }

    [Fact]
    public async Task GetUserByIdAsync_WithValidId_ShouldReturnUserResponse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = TestDataHelper.CreateTestUser();
        user = User.CreateFromDatabase(userId, user.Name, user.Email, user.PasswordHash, user.Role, user.CreatedAt);

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.GetUserByIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        Assert.Equal(user.Name, result.Name);
        Assert.Equal(user.Email.ToString(), result.Email);
        Assert.Equal(user.Role, result.Role);
        Assert.Equal(user.CreatedAt, result.CreatedAt);

        _userRepositoryMock.Verify(x => x.GetByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetUserByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _authService.GetUserByIdAsync(userId);

        // Assert
        Assert.Null(result);
        _userRepositoryMock.Verify(x => x.GetByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GenerateTokenAsync_ShouldReturnValidJwtToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@example.com";
        var role = UserRole.Manager.ToString();

        // Act
        var result = await _authService.GenerateTokenAsync(userId, email, role);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains(".", result); // JWT debe tener puntos separadores
    }
}
