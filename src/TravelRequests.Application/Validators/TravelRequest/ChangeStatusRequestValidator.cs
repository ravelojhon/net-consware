using FluentValidation;
using TravelRequests.Application.DTOs.TravelRequest;
using TravelRequests.Domain.Enums;

namespace TravelRequests.Application.Validators.TravelRequest;

public class ChangeStatusRequestValidator : AbstractValidator<ChangeStatusRequest>
{
    public ChangeStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid status specified")
            .Must(status => status == TravelRequestStatus.Approved || status == TravelRequestStatus.Rejected)
            .WithMessage("Only Approved or Rejected status changes are allowed");

        RuleFor(x => x.RejectionReason)
            .NotEmpty().When(x => x.Status == TravelRequestStatus.Rejected)
            .WithMessage("Rejection reason is required when rejecting a travel request")
            .MaximumLength(500).WithMessage("Rejection reason cannot exceed 500 characters")
            .Matches(@"^[a-zA-Z0-9\s\-'\.\,\!\?\(\)]+$")
            .When(x => !string.IsNullOrEmpty(x.RejectionReason))
            .WithMessage("Rejection reason contains invalid characters");
    }

}
