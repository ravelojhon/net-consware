using System.ComponentModel.DataAnnotations;

namespace TravelRequests.Application.DTOs.Auth;

public record RequestPasswordResetRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; init; } = string.Empty;
}
