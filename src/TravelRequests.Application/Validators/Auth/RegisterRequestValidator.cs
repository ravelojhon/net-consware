using FluentValidation;
using TravelRequests.Application.DTOs.Auth;

namespace TravelRequests.Application.Validators.Auth;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .Length(2, 100).WithMessage("Name must be between 2 and 100 characters")
            .Matches(@"^[a-zA-Z\s]+$").WithMessage("Name can only contain letters and spaces");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255).WithMessage("Email cannot exceed 255 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .Length(6, 100).WithMessage("Password must be between 6 and 100 characters")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]).+$")
            .WithMessage("Password must contain at least one lowercase letter, one uppercase letter, one digit, and one special character");

        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("Invalid role specified");
    }
}
