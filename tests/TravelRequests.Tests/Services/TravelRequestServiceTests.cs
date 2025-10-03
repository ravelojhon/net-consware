using Moq;
using TravelRequests.Application.Contracts.Repositories;
using TravelRequests.Application.DTOs.TravelRequest;
using TravelRequests.Application.Services;
using TravelRequests.Domain.Entities;
using TravelRequests.Domain.Enums;
using TravelRequests.Tests.Helpers;

namespace TravelRequests.Tests.Services;

public class TravelRequestServiceTests
{
    private readonly Mock<ITravelRequestRepository> _travelRequestRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly TravelRequestService _travelRequestService;

    public TravelRequestServiceTests()
    {
        _travelRequestRepositoryMock = new Mock<ITravelRequestRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();

        _travelRequestService = new TravelRequestService(
            _travelRequestRepositoryMock.Object,
            _userRepositoryMock.Object
        );
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldReturnTravelRequestResponse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new CreateTravelRequestRequest
        {
            OriginCity = "Bogotá",
            DestinationCity = "Medellín",
            DateFrom = DateTime.UtcNow.AddDays(30),
            DateTo = DateTime.UtcNow.AddDays(33),
            Justification = "Reunión de trabajo importante"
        };

        var user = TestDataHelper.CreateTestUser();
        user = User.CreateFromDatabase(userId, user.Name, user.Email, user.PasswordHash, user.Role, user.CreatedAt);

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _travelRequestRepositoryMock.Setup(x => x.AddAsync(It.IsAny<TravelRequest>()))
            .ReturnsAsync((TravelRequest request) => request);

        // Act
        var result = await _travelRequestService.CreateAsync(request, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.OriginCity, result.OriginCity);
        Assert.Equal(request.DestinationCity, result.DestinationCity);
        Assert.Equal(request.DateFrom, result.DateFrom);
        Assert.Equal(request.DateTo, result.DateTo);
        Assert.Equal(request.Justification, result.Justification);
        Assert.Equal(TravelRequestStatus.Pending, result.Status);

        _travelRequestRepositoryMock.Verify(x => x.AddAsync(It.IsAny<TravelRequest>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithSameOriginAndDestination_ShouldThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new CreateTravelRequestRequest
        {
            OriginCity = "Bogotá",
            DestinationCity = "Bogotá", // Misma ciudad
            DateFrom = DateTime.UtcNow.AddDays(30),
            DateTo = DateTime.UtcNow.AddDays(33),
            Justification = "Reunión de trabajo importante"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _travelRequestService.CreateAsync(request, userId));

        _travelRequestRepositoryMock.Verify(x => x.AddAsync(It.IsAny<TravelRequest>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidDateRange_ShouldThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new CreateTravelRequestRequest
        {
            OriginCity = "Bogotá",
            DestinationCity = "Medellín",
            DateFrom = DateTime.UtcNow.AddDays(33), // Fecha de fin antes que inicio
            DateTo = DateTime.UtcNow.AddDays(30),
            Justification = "Reunión de trabajo importante"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _travelRequestService.CreateAsync(request, userId));

        _travelRequestRepositoryMock.Verify(x => x.AddAsync(It.IsAny<TravelRequest>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithPastDate_ShouldThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new CreateTravelRequestRequest
        {
            OriginCity = "Bogotá",
            DestinationCity = "Medellín",
            DateFrom = DateTime.UtcNow.AddDays(-1), // Fecha en el pasado
            DateTo = DateTime.UtcNow.AddDays(2),
            Justification = "Reunión de trabajo importante"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _travelRequestService.CreateAsync(request, userId));

        _travelRequestRepositoryMock.Verify(x => x.AddAsync(It.IsAny<TravelRequest>()), Times.Never);
    }

    [Fact]
    public async Task ChangeStatusAsync_WithApproval_ShouldApproveTravelRequest()
    {
        // Arrange
        var travelRequestId = Guid.NewGuid();
        var approverId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var travelRequest = new TravelRequest(
            userId,
            "Bogotá",
            "Medellín",
            DateTime.UtcNow.AddDays(30),
            DateTime.UtcNow.AddDays(33),
            "Reunión de trabajo"
        );

        var changeStatusRequest = new ChangeStatusRequest
        {
            Status = TravelRequestStatus.Approved
        };

        var approver = TestDataHelper.CreateTestUser();
        approver = User.CreateFromDatabase(approverId, approver.Name, approver.Email, approver.PasswordHash, UserRole.Manager, approver.CreatedAt);

        _travelRequestRepositoryMock.Setup(x => x.GetWithUserAsync(travelRequestId))
            .ReturnsAsync(travelRequest);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(approverId))
            .ReturnsAsync(approver);
        _travelRequestRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<TravelRequest>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _travelRequestService.ChangeStatusAsync(travelRequestId, changeStatusRequest, approverId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(TravelRequestStatus.Approved, result.Status);
        Assert.Equal(approverId, result.ApprovedBy);

        _travelRequestRepositoryMock.Verify(x => x.GetWithUserAsync(travelRequestId), Times.Once);
        _travelRequestRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<TravelRequest>()), Times.Once);
    }

    [Fact]
    public async Task ChangeStatusAsync_WithRejection_ShouldRejectTravelRequest()
    {
        // Arrange
        var travelRequestId = Guid.NewGuid();
        var approverId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var travelRequest = new TravelRequest(
            userId,
            "Bogotá",
            "Medellín",
            DateTime.UtcNow.AddDays(30),
            DateTime.UtcNow.AddDays(33),
            "Reunión de trabajo"
        );

        var changeStatusRequest = new ChangeStatusRequest
        {
            Status = TravelRequestStatus.Rejected,
            RejectionReason = "No hay presupuesto disponible"
        };

        var approver = TestDataHelper.CreateTestUser();
        approver = User.CreateFromDatabase(approverId, approver.Name, approver.Email, approver.PasswordHash, UserRole.Manager, approver.CreatedAt);

        _travelRequestRepositoryMock.Setup(x => x.GetWithUserAsync(travelRequestId))
            .ReturnsAsync(travelRequest);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(approverId))
            .ReturnsAsync(approver);
        _travelRequestRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<TravelRequest>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _travelRequestService.ChangeStatusAsync(travelRequestId, changeStatusRequest, approverId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(TravelRequestStatus.Rejected, result.Status);
        Assert.Equal(changeStatusRequest.RejectionReason, result.RejectionReason);
        Assert.Equal(approverId, result.ApprovedBy);

        _travelRequestRepositoryMock.Verify(x => x.GetWithUserAsync(travelRequestId), Times.Once);
        _travelRequestRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<TravelRequest>()), Times.Once);
    }

    [Fact]
    public async Task ChangeStatusAsync_WithRejectionWithoutReason_ShouldThrowException()
    {
        // Arrange
        var travelRequestId = Guid.NewGuid();
        var approverId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var travelRequest = new TravelRequest(
            userId,
            "Bogotá",
            "Medellín",
            DateTime.UtcNow.AddDays(30),
            DateTime.UtcNow.AddDays(33),
            "Reunión de trabajo"
        );

        var changeStatusRequest = new ChangeStatusRequest
        {
            Status = TravelRequestStatus.Rejected,
            RejectionReason = null // Sin razón de rechazo
        };

        _travelRequestRepositoryMock.Setup(x => x.GetWithUserAsync(travelRequestId))
            .ReturnsAsync(travelRequest);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _travelRequestService.ChangeStatusAsync(travelRequestId, changeStatusRequest, approverId));

        _travelRequestRepositoryMock.Verify(x => x.GetWithUserAsync(travelRequestId), Times.Once);
        _travelRequestRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<TravelRequest>()), Times.Never);
    }

    [Fact]
    public async Task ChangeStatusAsync_WithNonPendingRequest_ShouldThrowException()
    {
        // Arrange
        var travelRequestId = Guid.NewGuid();
        var approverId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var travelRequest = new TravelRequest(
            userId,
            "Bogotá",
            "Medellín",
            DateTime.UtcNow.AddDays(30),
            DateTime.UtcNow.AddDays(33),
            "Reunión de trabajo"
        );

        // Cambiar el estado a aprobado
        travelRequest.Approve(approverId);

        var changeStatusRequest = new ChangeStatusRequest
        {
            Status = TravelRequestStatus.Rejected
        };

        _travelRequestRepositoryMock.Setup(x => x.GetWithUserAsync(travelRequestId))
            .ReturnsAsync(travelRequest);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _travelRequestService.ChangeStatusAsync(travelRequestId, changeStatusRequest, approverId));

        _travelRequestRepositoryMock.Verify(x => x.GetWithUserAsync(travelRequestId), Times.Once);
        _travelRequestRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<TravelRequest>()), Times.Never);
    }

    [Fact]
    public async Task ChangeStatusAsync_WithNonExistentRequest_ShouldThrowException()
    {
        // Arrange
        var travelRequestId = Guid.NewGuid();
        var approverId = Guid.NewGuid();

        var changeStatusRequest = new ChangeStatusRequest
        {
            Status = TravelRequestStatus.Approved
        };

        _travelRequestRepositoryMock.Setup(x => x.GetWithUserAsync(travelRequestId))
            .ReturnsAsync((TravelRequest?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _travelRequestService.ChangeStatusAsync(travelRequestId, changeStatusRequest, approverId));

        _travelRequestRepositoryMock.Verify(x => x.GetWithUserAsync(travelRequestId), Times.Once);
        _travelRequestRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<TravelRequest>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidIdAndOwner_ShouldReturnTravelRequest()
    {
        // Arrange
        var travelRequestId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var isApprover = false;

        var travelRequest = new TravelRequest(
            userId,
            "Bogotá",
            "Medellín",
            DateTime.UtcNow.AddDays(30),
            DateTime.UtcNow.AddDays(33),
            "Reunión de trabajo"
        );

        var user = TestDataHelper.CreateTestUser();
        user = User.CreateFromDatabase(userId, user.Name, user.Email, user.PasswordHash, user.Role, user.CreatedAt);

        _travelRequestRepositoryMock.Setup(x => x.GetWithUserAsync(travelRequestId))
            .ReturnsAsync(travelRequest);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _travelRequestService.GetByIdAsync(travelRequestId, userId, isApprover);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(travelRequest.OriginCity, result.OriginCity);
        Assert.Equal(travelRequest.DestinationCity, result.DestinationCity);
        Assert.Equal(travelRequest.Status, result.Status);

        _travelRequestRepositoryMock.Verify(x => x.GetWithUserAsync(travelRequestId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidIdAndApprover_ShouldReturnTravelRequest()
    {
        // Arrange
        var travelRequestId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var approverId = Guid.NewGuid();
        var isApprover = true;

        var travelRequest = new TravelRequest(
            userId,
            "Bogotá",
            "Medellín",
            DateTime.UtcNow.AddDays(30),
            DateTime.UtcNow.AddDays(33),
            "Reunión de trabajo"
        );

        var user = TestDataHelper.CreateTestUser();
        user = User.CreateFromDatabase(userId, user.Name, user.Email, user.PasswordHash, user.Role, user.CreatedAt);

        _travelRequestRepositoryMock.Setup(x => x.GetWithUserAsync(travelRequestId))
            .ReturnsAsync(travelRequest);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _travelRequestService.GetByIdAsync(travelRequestId, approverId, isApprover);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(travelRequest.OriginCity, result.OriginCity);
        Assert.Equal(travelRequest.DestinationCity, result.DestinationCity);
        Assert.Equal(travelRequest.Status, result.Status);

        _travelRequestRepositoryMock.Verify(x => x.GetWithUserAsync(travelRequestId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithUnauthorizedUser_ShouldThrowException()
    {
        // Arrange
        var travelRequestId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var isApprover = false;

        var travelRequest = new TravelRequest(
            otherUserId, // Diferente usuario
            "Bogotá",
            "Medellín",
            DateTime.UtcNow.AddDays(30),
            DateTime.UtcNow.AddDays(33),
            "Reunión de trabajo"
        );

        _travelRequestRepositoryMock.Setup(x => x.GetWithUserAsync(travelRequestId))
            .ReturnsAsync(travelRequest);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
            _travelRequestService.GetByIdAsync(travelRequestId, userId, isApprover));

        _travelRequestRepositoryMock.Verify(x => x.GetWithUserAsync(travelRequestId), Times.Once);
    }
}
