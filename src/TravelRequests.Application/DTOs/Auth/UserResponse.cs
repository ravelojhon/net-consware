using TravelRequests.Domain.Enums;

namespace TravelRequests.Application.DTOs.Auth;

public record UserResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public UserRole Role { get; init; }
    public DateTime CreatedAt { get; init; }
}
