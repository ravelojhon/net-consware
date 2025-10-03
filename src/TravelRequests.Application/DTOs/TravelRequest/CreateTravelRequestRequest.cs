using System.ComponentModel.DataAnnotations;

namespace TravelRequests.Application.DTOs.TravelRequest;

public record CreateTravelRequestRequest
{
    [Required(ErrorMessage = "Origin city is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Origin city must be between 2 and 100 characters")]
    public string OriginCity { get; init; } = string.Empty;

    [Required(ErrorMessage = "Destination city is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Destination city must be between 2 and 100 characters")]
    public string DestinationCity { get; init; } = string.Empty;

    [Required(ErrorMessage = "Start date is required")]
    public DateTime DateFrom { get; init; }

    [Required(ErrorMessage = "End date is required")]
    public DateTime DateTo { get; init; }

    [Required(ErrorMessage = "Justification is required")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "Justification must be between 10 and 1000 characters")]
    public string Justification { get; init; } = string.Empty;
}
