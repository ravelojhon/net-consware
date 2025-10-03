using System.ComponentModel.DataAnnotations;
using TravelRequests.Domain.Enums;

namespace TravelRequests.Application.DTOs.Auth;

public record RegisterRequest
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
    public string Name { get; init; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
    public string Password { get; init; } = string.Empty;

    [Required(ErrorMessage = "Role is required")]
    public UserRole Role { get; init; }
}
