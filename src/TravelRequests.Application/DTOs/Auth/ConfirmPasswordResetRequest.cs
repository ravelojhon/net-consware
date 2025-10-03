using System.ComponentModel.DataAnnotations;

namespace TravelRequests.Application.DTOs.Auth;

public record ConfirmPasswordResetRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage = "Code is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Code is required")]
    public string Code { get; init; } = string.Empty;

    [Required(ErrorMessage = "New password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "New password must be between 6 and 100 characters")]
    public string NewPassword { get; init; } = string.Empty;
}
