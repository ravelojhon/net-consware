namespace TravelRequests.Application.DTOs.Auth;

public record PasswordResetResponse
{
    public string Email { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
}
