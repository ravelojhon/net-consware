using TravelRequests.Domain.Enums;

namespace TravelRequests.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public UserRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation properties
    public ICollection<TravelRequest> TravelRequests { get; private set; } = new List<TravelRequest>();
    public ICollection<TravelRequest> ApprovedTravelRequests { get; private set; } = new List<TravelRequest>();
    public ICollection<PasswordResetCode> PasswordResetCodes { get; private set; } = new List<PasswordResetCode>();

    // Constructor privado para EF Core
    private User() { }

    // Constructor para crear nuevos usuarios
    public User(string name, string email, string passwordHash, UserRole role)
    {
        ValidateInputs(name, email, passwordHash);
        
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        CreatedAt = DateTime.UtcNow;
    }

    // Método de fábrica para crear usuario desde base de datos
    public static User CreateFromDatabase(Guid id, string name, string email, string passwordHash, 
        UserRole role, DateTime createdAt, DateTime? updatedAt = null)
    {
        return new User
        {
            Id = id,
            Name = name,
            Email = email,
            PasswordHash = passwordHash,
            Role = role,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
    }

    // Métodos de negocio
    public void UpdateProfile(string name, string email)
    {
        ValidateInputs(name, email, PasswordHash);
        
        Name = name;
        Email = email;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Password hash cannot be null or empty", nameof(newPasswordHash));
            
        PasswordHash = newPasswordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeRole(UserRole newRole)
    {
        Role = newRole;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateInputs(string name, string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));
            
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty", nameof(email));
            
        if (!IsValidEmail(email))
            throw new ArgumentException("Invalid email format", nameof(email));
            
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be null or empty", nameof(passwordHash));
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
