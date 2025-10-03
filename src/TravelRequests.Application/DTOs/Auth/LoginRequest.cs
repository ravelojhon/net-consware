using System.ComponentModel.DataAnnotations;

namespace TravelRequests.Application.DTOs.Auth;

public record LoginRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Password is required")]
    public string Password { get; init; } = string.Empty;
}
