using TravelRequests.Application.DTOs.Auth;

namespace TravelRequests.Application.Contracts.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<UserResponse?> GetUserByIdAsync(Guid userId);
    Task<bool> ValidateTokenAsync(string token);
    Task<string> GenerateTokenAsync(Guid userId, string email, string role);
}
