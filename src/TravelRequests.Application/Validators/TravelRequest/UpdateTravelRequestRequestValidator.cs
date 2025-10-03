using FluentValidation;
using TravelRequests.Application.DTOs.TravelRequest;

namespace TravelRequests.Application.Validators.TravelRequest;

public class UpdateTravelRequestRequestValidator : AbstractValidator<UpdateTravelRequestRequest>
{
    public UpdateTravelRequestRequestValidator()
    {
        RuleFor(x => x.OriginCity)
            .NotEmpty().WithMessage("Origin city is required")
            .Length(2, 100).WithMessage("Origin city must be between 2 and 100 characters")
            .Matches(@"^[a-zA-Z\s\-'\.]+$").WithMessage("Origin city can only contain letters, spaces, hyphens, apostrophes, and periods");

        RuleFor(x => x.DestinationCity)
            .NotEmpty().WithMessage("Destination city is required")
            .Length(2, 100).WithMessage("Destination city must be between 2 and 100 characters")
            .Matches(@"^[a-zA-Z\s\-'\.]+$").WithMessage("Destination city can only contain letters, spaces, hyphens, apostrophes, and periods");

        RuleFor(x => x.DateFrom)
            .NotEmpty().WithMessage("Start date is required")
            .Must(BeInTheFuture).WithMessage("Start date cannot be in the past")
            .Must(BeWithinReasonableRange).WithMessage("Start date must be within the next 2 years");

        RuleFor(x => x.DateTo)
            .NotEmpty().WithMessage("End date is required")
            .Must(BeInTheFuture).WithMessage("End date cannot be in the past")
            .Must(BeWithinReasonableRange).WithMessage("End date must be within the next 2 years");

        RuleFor(x => x)
            .Must(HaveValidDateRange).WithMessage("End date must be after start date")
            .Must(HaveDifferentCities).WithMessage("Origin and destination cities must be different");

        RuleFor(x => x.Justification)
            .NotEmpty().WithMessage("Justification is required")
            .Length(10, 1000).WithMessage("Justification must be between 10 and 1000 characters")
            .Matches(@"^[a-zA-Z0-9\s\-'\.\,\!\?\(\)]+$").WithMessage("Justification contains invalid characters");
    }

    private static bool BeInTheFuture(DateTime date)
    {
        return date >= DateTime.Today;
    }

    private static bool BeWithinReasonableRange(DateTime date)
    {
        return date <= DateTime.Today.AddYears(2);
    }

    private static bool HaveValidDateRange(UpdateTravelRequestRequest request)
    {
        return request.DateFrom < request.DateTo;
    }

    private static bool HaveDifferentCities(UpdateTravelRequestRequest request)
    {
        return !string.Equals(request.OriginCity, request.DestinationCity, StringComparison.OrdinalIgnoreCase);
    }
}
