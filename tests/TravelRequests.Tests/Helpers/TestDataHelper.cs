using TravelRequests.Application.DTOs.Auth;
using TravelRequests.Domain.Entities;
using TravelRequests.Domain.Enums;
using TravelRequests.Domain.ValueObjects;

namespace TravelRequests.Tests.Helpers;

public static class TestDataHelper
{
    public static User CreateTestUser(string email = "test@example.com", UserRole role = UserRole.Employee)
    {
        return new User(
            "Test User",
            new Email(email),
            BCrypt.Net.BCrypt.HashPassword("TestPassword123!"),
            role
        );
    }

    public static RegisterRequest CreateValidRegisterRequest()
    {
        return new RegisterRequest
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "TestPassword123!",
            Role = UserRole.Employee
        };
    }

    public static LoginRequest CreateValidLoginRequest()
    {
        return new LoginRequest
        {
            Email = "test@example.com",
            Password = "TestPassword123!"
        };
    }

    public static RequestPasswordResetRequest CreatePasswordResetRequest()
    {
        return new RequestPasswordResetRequest
        {
            Email = "test@example.com"
        };
    }

    public static ConfirmPasswordResetRequest CreateConfirmPasswordResetRequest()
    {
        return new ConfirmPasswordResetRequest
        {
            Email = "test@example.com",
            Code = "123456",
            NewPassword = "NewPassword123!"
        };
    }

    public static PasswordResetCode CreatePasswordResetCode(Guid userId, string code = "123456")
    {
        return new PasswordResetCode(userId, code, DateTime.UtcNow.AddMinutes(5));
    }
}
