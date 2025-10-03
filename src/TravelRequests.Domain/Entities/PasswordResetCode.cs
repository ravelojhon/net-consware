namespace TravelRequests.Domain.Entities;

public class PasswordResetCode
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Code { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsUsed { get; private set; }
    public DateTime? UsedAt { get; private set; }

    // Constructor privado para EF Core
    private PasswordResetCode() { }

    // Constructor para crear nuevos códigos de reset
    public PasswordResetCode(Guid userId, string code, DateTime expiresAt)
    {
        ValidateInputs(userId, code, expiresAt);
        
        Id = Guid.NewGuid();
        UserId = userId;
        Code = code;
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.UtcNow;
        IsUsed = false;
    }

    // Método de fábrica para crear código desde base de datos
    public static PasswordResetCode CreateFromDatabase(Guid id, Guid userId, string code, 
        DateTime expiresAt, DateTime createdAt, bool isUsed, DateTime? usedAt = null)
    {
        return new PasswordResetCode
        {
            Id = id,
            UserId = userId,
            Code = code,
            ExpiresAt = expiresAt,
            CreatedAt = createdAt,
            IsUsed = isUsed,
            UsedAt = usedAt
        };
    }

    // Métodos de negocio
    public bool IsValid()
    {
        return !IsUsed && DateTime.UtcNow <= ExpiresAt;
    }

    public void MarkAsUsed()
    {
        if (IsUsed)
            throw new InvalidOperationException("Code has already been used");
            
        if (!IsValid())
            throw new InvalidOperationException("Code has expired");
            
        IsUsed = true;
        UsedAt = DateTime.UtcNow;
    }

    public void ExtendExpiration(DateTime newExpirationTime)
    {
        if (IsUsed)
            throw new InvalidOperationException("Cannot extend expiration of used code");
            
        if (newExpirationTime <= DateTime.UtcNow)
            throw new ArgumentException("New expiration time must be in the future", nameof(newExpirationTime));
            
        ExpiresAt = newExpirationTime;
    }

    private static void ValidateInputs(Guid userId, string code, DateTime expiresAt)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
            
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code cannot be null or empty", nameof(code));
            
        if (code.Length < 6)
            throw new ArgumentException("Code must be at least 6 characters long", nameof(code));
            
        if (expiresAt <= DateTime.UtcNow)
            throw new ArgumentException("Expiration time must be in the future", nameof(expiresAt));
    }
}
