using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TravelRequests.Application.DTOs.Auth;
using TravelRequests.Application.DTOs.TravelRequest;
using TravelRequests.Application.Services;
using TravelRequests.Domain.Entities;
using TravelRequests.Domain.Enums;
using TravelRequests.Domain.ValueObjects;
using TravelRequests.Infrastructure.Persistence;
using TravelRequests.Infrastructure.Repositories;
using TravelRequests.Tests.Helpers;

namespace TravelRequests.Tests.Services;

public class IntegrationTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly AuthService _authService;
    private readonly TravelRequestService _travelRequestService;

    public IntegrationTests()
    {
        // Configurar InMemory Database
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);

        // Configurar configuración mock
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "TU_SUPER_SECRETO_AQUI_MUY_LARGO_Y_SEGURO_2025",
                ["Jwt:Issuer"] = "TravelRequestsApi",
                ["Jwt:Audience"] = "TravelRequestsClient",
                ["Jwt:ExpirationMinutes"] = "60"
            })
            .Build();

        // Crear servicios con repositorios reales
        var userRepository = new UserRepository(_context);
        var travelRequestRepository = new TravelRequestRepository(_context);
        var passwordResetCodeRepository = new PasswordResetCodeRepository(_context);

        _authService = new AuthService(userRepository, passwordResetCodeRepository, configuration);
        _travelRequestService = new TravelRequestService(travelRequestRepository, userRepository);
    }

    [Fact]
    public async Task FullWorkflow_RegisterLoginCreateTravelRequest_ShouldWork()
    {
        // Arrange
        var registerRequest = TestDataHelper.CreateValidRegisterRequest();

        // Act 1: Register
        var authResponse = await _authService.RegisterAsync(registerRequest);
        Assert.NotNull(authResponse);
        Assert.NotEmpty(authResponse.Token);

        // Act 2: Login
        var loginRequest = TestDataHelper.CreateValidLoginRequest();
        var loginResponse = await _authService.LoginAsync(loginRequest);
        Assert.NotNull(loginResponse);
        Assert.Equal(authResponse.UserId, loginResponse.UserId);

        // Act 3: Create Travel Request
        var createRequest = new CreateTravelRequestRequest
        {
            OriginCity = "Bogotá",
            DestinationCity = "Medellín",
            DateFrom = DateTime.UtcNow.AddDays(30),
            DateTo = DateTime.UtcNow.AddDays(33),
            Justification = "Reunión de trabajo importante"
        };

        var travelRequest = await _travelRequestService.CreateAsync(createRequest, Guid.Parse(authResponse.UserId));
        Assert.NotNull(travelRequest);
        Assert.Equal(TravelRequestStatus.Pending, travelRequest.Status);

        // Act 4: Get Travel Requests
        var userTravelRequests = await _travelRequestService.GetByUserIdAsync(Guid.Parse(authResponse.UserId));
        Assert.Single(userTravelRequests);
        Assert.Equal(travelRequest.Id, userTravelRequests.First().Id);
    }

    [Fact]
    public async Task PasswordResetWorkflow_ShouldWork()
    {
        // Arrange
        var registerRequest = TestDataHelper.CreateValidRegisterRequest();
        var authResponse = await _authService.RegisterAsync(registerRequest);

        // Act 1: Request Password Reset
        var resetRequest = TestDataHelper.CreatePasswordResetRequest();
        var resetResponse = await _authService.RequestPasswordResetAsync(resetRequest);
        Assert.NotNull(resetResponse);
        Assert.Equal(registerRequest.Email, resetResponse.Email);
        Assert.NotEmpty(resetResponse.Code);

        // Act 2: Confirm Password Reset
        var confirmRequest = new ConfirmPasswordResetRequest
        {
            Email = registerRequest.Email,
            Code = resetResponse.Code,
            NewPassword = "NewPassword123!"
        };

        await _authService.ConfirmPasswordResetAsync(confirmRequest);

        // Act 3: Login with New Password
        var loginRequest = new LoginRequest
        {
            Email = registerRequest.Email,
            Password = "NewPassword123!"
        };

        var loginResponse = await _authService.LoginAsync(loginRequest);
        Assert.NotNull(loginResponse);
        Assert.Equal(authResponse.UserId, loginResponse.UserId);
    }

    [Fact]
    public async Task ApprovalWorkflow_ShouldWork()
    {
        // Arrange - Create two users: one employee and one manager
        var employeeRequest = TestDataHelper.CreateValidRegisterRequest();
        employeeRequest = employeeRequest with { Email = "employee@example.com", Role = UserRole.Employee };
        var employeeAuth = await _authService.RegisterAsync(employeeRequest);

        var managerRequest = TestDataHelper.CreateValidRegisterRequest();
        managerRequest = managerRequest with { Email = "manager@example.com", Role = UserRole.Manager };
        var managerAuth = await _authService.RegisterAsync(managerRequest);

        // Act 1: Employee creates travel request
        var createRequest = new CreateTravelRequestRequest
        {
            OriginCity = "Bogotá",
            DestinationCity = "Medellín",
            DateFrom = DateTime.UtcNow.AddDays(30),
            DateTo = DateTime.UtcNow.AddDays(33),
            Justification = "Reunión de trabajo importante"
        };

        var travelRequest = await _travelRequestService.CreateAsync(createRequest, Guid.Parse(employeeAuth.UserId));
        Assert.Equal(TravelRequestStatus.Pending, travelRequest.Status);

        // Act 2: Manager approves the request
        var changeStatusRequest = new ChangeStatusRequest
        {
            Status = TravelRequestStatus.Approved
        };

        var approvedRequest = await _travelRequestService.ChangeStatusAsync(
            travelRequest.Id, 
            changeStatusRequest, 
            Guid.Parse(managerAuth.UserId));

        Assert.Equal(TravelRequestStatus.Approved, approvedRequest.Status);
        Assert.Equal(Guid.Parse(managerAuth.UserId), approvedRequest.ApprovedBy);
        Assert.NotNull(approvedRequest.ApprovedAt);
    }

    [Fact]
    public async Task RejectionWorkflow_ShouldWork()
    {
        // Arrange - Create two users: one employee and one manager
        var employeeRequest = TestDataHelper.CreateValidRegisterRequest();
        employeeRequest = employeeRequest with { Email = "employee2@example.com", Role = UserRole.Employee };
        var employeeAuth = await _authService.RegisterAsync(employeeRequest);

        var managerRequest = TestDataHelper.CreateValidRegisterRequest();
        managerRequest = managerRequest with { Email = "manager2@example.com", Role = UserRole.Manager };
        var managerAuth = await _authService.RegisterAsync(managerRequest);

        // Act 1: Employee creates travel request
        var createRequest = new CreateTravelRequestRequest
        {
            OriginCity = "Bogotá",
            DestinationCity = "Medellín",
            DateFrom = DateTime.UtcNow.AddDays(30),
            DateTo = DateTime.UtcNow.AddDays(33),
            Justification = "Reunión de trabajo importante"
        };

        var travelRequest = await _travelRequestService.CreateAsync(createRequest, Guid.Parse(employeeAuth.UserId));
        Assert.Equal(TravelRequestStatus.Pending, travelRequest.Status);

        // Act 2: Manager rejects the request
        var changeStatusRequest = new ChangeStatusRequest
        {
            Status = TravelRequestStatus.Rejected,
            RejectionReason = "No hay presupuesto disponible para este viaje"
        };

        var rejectedRequest = await _travelRequestService.ChangeStatusAsync(
            travelRequest.Id, 
            changeStatusRequest, 
            Guid.Parse(managerAuth.UserId));

        Assert.Equal(TravelRequestStatus.Rejected, rejectedRequest.Status);
        Assert.Equal(changeStatusRequest.RejectionReason, rejectedRequest.RejectionReason);
        Assert.Equal(Guid.Parse(managerAuth.UserId), rejectedRequest.ApprovedBy);
        Assert.NotNull(rejectedRequest.ApprovedAt);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
