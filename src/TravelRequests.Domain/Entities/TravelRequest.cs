using TravelRequests.Domain.Enums;

namespace TravelRequests.Domain.Entities;

public class TravelRequest
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string OriginCity { get; private set; }
    public string DestinationCity { get; private set; }
    public DateTime DateFrom { get; private set; }
    public DateTime DateTo { get; private set; }
    public string Justification { get; private set; }
    public TravelRequestStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public string? RejectionReason { get; private set; }
    public Guid? ApprovedBy { get; private set; }
    public DateTime? ApprovedAt { get; private set; }

    // Navigation properties
    public User User { get; private set; } = null!;
    public User? ApprovedByUser { get; private set; }

    // Constructor privado para EF Core
    private TravelRequest() { }

    // Constructor para crear nuevas solicitudes
    public TravelRequest(Guid userId, string originCity, string destinationCity,
        DateTime dateFrom, DateTime dateTo, string justification)
    {
        ValidateInputs(userId, originCity, destinationCity, dateFrom, dateTo, justification);

        Id = Guid.NewGuid();
        UserId = userId;
        OriginCity = originCity;
        DestinationCity = destinationCity;
        DateFrom = dateFrom;
        DateTo = dateTo;
        Justification = justification;
        Status = TravelRequestStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    // Método de fábrica para crear solicitud desde base de datos
    public static TravelRequest CreateFromDatabase(Guid id, Guid userId, string originCity,
        string destinationCity, DateTime dateFrom, DateTime dateTo, string justification,
        TravelRequestStatus status, DateTime createdAt, DateTime? updatedAt = null,
        string? rejectionReason = null, Guid? approvedBy = null, DateTime? approvedAt = null)
    {
        return new TravelRequest
        {
            Id = id,
            UserId = userId,
            OriginCity = originCity,
            DestinationCity = destinationCity,
            DateFrom = dateFrom,
            DateTo = dateTo,
            Justification = justification,
            Status = status,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            RejectionReason = rejectionReason,
            ApprovedBy = approvedBy,
            ApprovedAt = approvedAt
        };
    }

    // Métodos de negocio
    public void Approve(Guid approvedBy)
    {
        if (Status != TravelRequestStatus.Pending)
        {
            throw new InvalidOperationException("Only pending requests can be approved");
        }

        Status = TravelRequestStatus.Approved;
        ApprovedBy = approvedBy;
        ApprovedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject(string rejectionReason, Guid rejectedBy)
    {
        if (Status != TravelRequestStatus.Pending)
        {
            throw new InvalidOperationException("Only pending requests can be rejected");
        }

        if (string.IsNullOrWhiteSpace(rejectionReason))
        {
            throw new ArgumentException("Rejection reason cannot be null or empty", nameof(rejectionReason));
        }

        Status = TravelRequestStatus.Rejected;
        RejectionReason = rejectionReason;
        ApprovedBy = rejectedBy; // Usamos el mismo campo para quien rechaza
        ApprovedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == TravelRequestStatus.Approved)
        {
            throw new InvalidOperationException("Approved requests cannot be cancelled");
        }

        if (Status == TravelRequestStatus.Cancelled)
        {
            throw new InvalidOperationException("Request is already cancelled");
        }

        Status = TravelRequestStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string originCity, string destinationCity,
        DateTime dateFrom, DateTime dateTo, string justification)
    {
        if (Status != TravelRequestStatus.Pending)
        {
            throw new InvalidOperationException("Only pending requests can be updated");
        }

        ValidateInputs(UserId, originCity, destinationCity, dateFrom, dateTo, justification);

        OriginCity = originCity;
        DestinationCity = destinationCity;
        DateFrom = dateFrom;
        DateTo = dateTo;
        Justification = justification;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateInputs(Guid userId, string originCity, string destinationCity,
        DateTime dateFrom, DateTime dateTo, string justification)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
        }

        if (string.IsNullOrWhiteSpace(originCity))
        {
            throw new ArgumentException("Origin city cannot be null or empty", nameof(originCity));
        }

        if (string.IsNullOrWhiteSpace(destinationCity))
        {
            throw new ArgumentException("Destination city cannot be null or empty", nameof(destinationCity));
        }

        if (string.IsNullOrWhiteSpace(justification))
        {
            throw new ArgumentException("Justification cannot be null or empty", nameof(justification));
        }

        if (dateFrom < DateTime.UtcNow.Date)
        {
            throw new ArgumentException("Travel date cannot be in the past", nameof(dateFrom));
        }

        if (dateTo <= dateFrom)
        {
            throw new ArgumentException("Return date must be after departure date", nameof(dateTo));
        }

        if (dateTo > dateFrom.AddDays(365))
        {
            throw new ArgumentException("Travel duration cannot exceed 365 days", nameof(dateTo));
        }
    }
}
