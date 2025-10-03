using TravelRequests.Domain.Enums;

namespace TravelRequests.Application.DTOs.TravelRequest;

public record TravelRequestResponse
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string UserEmail { get; init; } = string.Empty;
    public string OriginCity { get; init; } = string.Empty;
    public string DestinationCity { get; init; } = string.Empty;
    public DateTime DateFrom { get; init; }
    public DateTime DateTo { get; init; }
    public string Justification { get; init; } = string.Empty;
    public TravelRequestStatus Status { get; init; }
    public string? RejectionReason { get; init; }
    public Guid? ApprovedBy { get; init; }
    public string? ApproverName { get; init; }
    public DateTime? ApprovedAt { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
