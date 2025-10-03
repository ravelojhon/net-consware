using FluentValidation;
using TravelRequests.Application.DTOs.Auth;

namespace TravelRequests.Application.Validators.Auth;

public class ConfirmPasswordResetRequestValidator : AbstractValidator<ConfirmPasswordResetRequest>
{
    public ConfirmPasswordResetRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255).WithMessage("Email cannot exceed 255 characters");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required")
            .Length(6, 50).WithMessage("Code must be between 6 and 50 characters")
            .Matches(@"^\d+$").WithMessage("Code must contain only digits");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required")
            .Length(6, 100).WithMessage("New password must be between 6 and 100 characters")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]).+$")
            .WithMessage("New password must contain at least one lowercase letter, one uppercase letter, one digit, and one special character");
    }
}
