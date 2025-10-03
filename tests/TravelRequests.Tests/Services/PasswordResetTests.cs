using Microsoft.Extensions.Configuration;
using Moq;
using TravelRequests.Application.Contracts.Repositories;
using TravelRequests.Application.DTOs.Auth;
using TravelRequests.Application.Services;
using TravelRequests.Domain.Entities;
using TravelRequests.Domain.Enums;
using TravelRequests.Tests.Helpers;

namespace TravelRequests.Tests.Services;

public class PasswordResetTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordResetCodeRepository> _passwordResetCodeRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly AuthService _authService;

    public PasswordResetTests()
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
    public async Task RequestPasswordResetAsync_WithValidEmail_ShouldReturnPasswordResetResponse()
    {
        // Arrange
        var request = TestDataHelper.CreatePasswordResetRequest();
        var user = TestDataHelper.CreateTestUser(request.Email);

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(request.Email))
            .ReturnsAsync(user);
        _passwordResetCodeRepositoryMock.Setup(x => x.AddAsync(It.IsAny<PasswordResetCode>()))
            .ReturnsAsync((PasswordResetCode code) => code);

        // Act
        var result = await _authService.RequestPasswordResetAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Email, result.Email);
        Assert.NotEmpty(result.Code);
        Assert.True(result.Code.Length == 6); // Código de 6 dígitos
        Assert.True(result.ExpiresAt > DateTime.UtcNow);
        Assert.True(result.ExpiresAt <= DateTime.UtcNow.AddMinutes(6)); // Máximo 6 minutos

        _userRepositoryMock.Verify(x => x.GetByEmailAsync(request.Email), Times.Once);
        _passwordResetCodeRepositoryMock.Verify(x => x.AddAsync(It.IsAny<PasswordResetCode>()), Times.Once);
    }

    [Fact]
    public async Task RequestPasswordResetAsync_WithInvalidEmail_ShouldThrowException()
    {
        // Arrange
        var request = TestDataHelper.CreatePasswordResetRequest();

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(request.Email))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _authService.RequestPasswordResetAsync(request));

        _userRepositoryMock.Verify(x => x.GetByEmailAsync(request.Email), Times.Once);
        _passwordResetCodeRepositoryMock.Verify(x => x.AddAsync(It.IsAny<PasswordResetCode>()), Times.Never);
    }

    [Fact]
    public async Task ConfirmPasswordResetAsync_WithValidCode_ShouldUpdatePassword()
    {
        // Arrange
        var request = TestDataHelper.CreateConfirmPasswordResetRequest();
        var user = TestDataHelper.CreateTestUser(request.Email);
        var resetCode = TestDataHelper.CreatePasswordResetCode(user.Id, request.Code);

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(request.Email))
            .ReturnsAsync(user);
        _passwordResetCodeRepositoryMock.Setup(x => x.GetByCodeAsync(request.Code))
            .ReturnsAsync(resetCode);
        _userRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);
        _passwordResetCodeRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<PasswordResetCode>()))
            .Returns(Task.CompletedTask);

        // Act
        await _authService.ConfirmPasswordResetAsync(request);

        // Assert
        _userRepositoryMock.Verify(x => x.GetByEmailAsync(request.Email), Times.Once);
        _passwordResetCodeRepositoryMock.Verify(x => x.GetByCodeAsync(request.Code), Times.Once);
        _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
        _passwordResetCodeRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<PasswordResetCode>()), Times.Once);
    }

    [Fact]
    public async Task ConfirmPasswordResetAsync_WithInvalidEmail_ShouldThrowException()
    {
        // Arrange
        var request = TestDataHelper.CreateConfirmPasswordResetRequest();

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(request.Email))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _authService.ConfirmPasswordResetAsync(request));

        _userRepositoryMock.Verify(x => x.GetByEmailAsync(request.Email), Times.Once);
        _passwordResetCodeRepositoryMock.Verify(x => x.GetByCodeAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ConfirmPasswordResetAsync_WithInvalidCode_ShouldThrowException()
    {
        // Arrange
        var request = TestDataHelper.CreateConfirmPasswordResetRequest();
        var user = TestDataHelper.CreateTestUser(request.Email);

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(request.Email))
            .ReturnsAsync(user);
        _passwordResetCodeRepositoryMock.Setup(x => x.GetByCodeAsync(request.Code))
            .ReturnsAsync((PasswordResetCode?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _authService.ConfirmPasswordResetAsync(request));

        _userRepositoryMock.Verify(x => x.GetByEmailAsync(request.Email), Times.Once);
        _passwordResetCodeRepositoryMock.Verify(x => x.GetByCodeAsync(request.Code), Times.Once);
    }

    [Fact]
    public async Task ConfirmPasswordResetAsync_WithExpiredCode_ShouldThrowException()
    {
        // Arrange
        var request = TestDataHelper.CreateConfirmPasswordResetRequest();
        var user = TestDataHelper.CreateTestUser(request.Email);
        var expiredCode = PasswordResetCode.CreateFromDatabase(
            Guid.NewGuid(), 
            user.Id, 
            request.Code, 
            DateTime.UtcNow.AddMinutes(-1), // Código expirado
            DateTime.UtcNow.AddMinutes(-2),
            false,
            null
        );

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(request.Email))
            .ReturnsAsync(user);
        _passwordResetCodeRepositoryMock.Setup(x => x.GetByCodeAsync(request.Code))
            .ReturnsAsync(expiredCode);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _authService.ConfirmPasswordResetAsync(request));

        _userRepositoryMock.Verify(x => x.GetByEmailAsync(request.Email), Times.Once);
        _passwordResetCodeRepositoryMock.Verify(x => x.GetByCodeAsync(request.Code), Times.Once);
    }

    [Fact]
    public async Task ConfirmPasswordResetAsync_WithUsedCode_ShouldThrowException()
    {
        // Arrange
        var request = TestDataHelper.CreateConfirmPasswordResetRequest();
        var user = TestDataHelper.CreateTestUser(request.Email);
        var usedCode = TestDataHelper.CreatePasswordResetCode(user.Id, request.Code);
        usedCode.MarkAsUsed(); // Marcar como usado

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(request.Email))
            .ReturnsAsync(user);
        _passwordResetCodeRepositoryMock.Setup(x => x.GetByCodeAsync(request.Code))
            .ReturnsAsync(usedCode);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _authService.ConfirmPasswordResetAsync(request));

        _userRepositoryMock.Verify(x => x.GetByEmailAsync(request.Email), Times.Once);
        _passwordResetCodeRepositoryMock.Verify(x => x.GetByCodeAsync(request.Code), Times.Once);
    }

    [Fact]
    public async Task ConfirmPasswordResetAsync_WithWrongUserCode_ShouldThrowException()
    {
        // Arrange
        var request = TestDataHelper.CreateConfirmPasswordResetRequest();
        var user = TestDataHelper.CreateTestUser(request.Email);
        var wrongUserCode = TestDataHelper.CreatePasswordResetCode(Guid.NewGuid(), request.Code); // Código de otro usuario

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(request.Email))
            .ReturnsAsync(user);
        _passwordResetCodeRepositoryMock.Setup(x => x.GetByCodeAsync(request.Code))
            .ReturnsAsync(wrongUserCode);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _authService.ConfirmPasswordResetAsync(request));

        _userRepositoryMock.Verify(x => x.GetByEmailAsync(request.Email), Times.Once);
        _passwordResetCodeRepositoryMock.Verify(x => x.GetByCodeAsync(request.Code), Times.Once);
    }
}
