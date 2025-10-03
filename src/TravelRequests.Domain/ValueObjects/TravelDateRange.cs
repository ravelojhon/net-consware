namespace TravelRequests.Domain.ValueObjects;

public record TravelDateRange
{
    public DateTime DateFrom { get; }
    public DateTime DateTo { get; }
    public int DurationInDays => (DateTo - DateFrom).Days;

    public TravelDateRange(DateTime dateFrom, DateTime dateTo)
    {
        if (dateFrom < DateTime.UtcNow.Date)
            throw new ArgumentException("Travel date cannot be in the past", nameof(dateFrom));
            
        if (dateTo <= dateFrom)
            throw new ArgumentException("Return date must be after departure date", nameof(dateTo));
            
        if (dateTo > dateFrom.AddDays(365))
            throw new ArgumentException("Travel duration cannot exceed 365 days", nameof(dateTo));
            
        DateFrom = dateFrom.Date;
        DateTo = dateTo.Date;
    }

    public bool IsInPast() => DateFrom < DateTime.UtcNow.Date;
    
    public bool IsInFuture() => DateFrom > DateTime.UtcNow.Date;
    
    public bool IsToday() => DateFrom.Date == DateTime.UtcNow.Date;
    
    public bool OverlapsWith(TravelDateRange other)
    {
        return DateFrom <= other.DateTo && other.DateFrom <= DateTo;
    }

    public override string ToString() => $"{DateFrom:yyyy-MM-dd} to {DateTo:yyyy-MM-dd}";
}
