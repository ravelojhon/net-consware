using TravelRequests.Domain.Enums;

namespace TravelRequests.Application.DTOs.TravelRequest;

public record TravelRequestListResponse
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string OriginCity { get; init; } = string.Empty;
    public string DestinationCity { get; init; } = string.Empty;
    public DateTime DateFrom { get; init; }
    public DateTime DateTo { get; init; }
    public TravelRequestStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
}
