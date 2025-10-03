using System.ComponentModel.DataAnnotations;
using TravelRequests.Domain.Enums;

namespace TravelRequests.Application.DTOs.TravelRequest;

public record ChangeStatusRequest
{
    [Required(ErrorMessage = "Status is required")]
    public TravelRequestStatus Status { get; init; }

    [StringLength(500, ErrorMessage = "Rejection reason cannot exceed 500 characters")]
    public string? RejectionReason { get; init; }
}
