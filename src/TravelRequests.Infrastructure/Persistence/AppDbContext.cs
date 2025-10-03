using Microsoft.EntityFrameworkCore;
using TravelRequests.Domain.Entities;

namespace TravelRequests.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSets
    public DbSet<User> Users { get; set; }
    public DbSet<TravelRequest> TravelRequests { get; set; }
    public DbSet<PasswordResetCode> PasswordResetCodes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurar User
        ConfigureUser(modelBuilder);
        
        // Configurar TravelRequest
        ConfigureTravelRequest(modelBuilder);
        
        // Configurar PasswordResetCode
        ConfigurePasswordResetCode(modelBuilder);
    }

    private void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);
                
            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);
                
            entity.Property(e => e.Role)
                .IsRequired()
                .HasConversion<int>();
                
            entity.Property(e => e.CreatedAt)
                .IsRequired();
                
            entity.Property(e => e.UpdatedAt);

            // Índice único para Email
            entity.HasIndex(e => e.Email)
                .IsUnique()
                .HasDatabaseName("IX_Users_Email");
                
            // Índice para búsquedas por Role
            entity.HasIndex(e => e.Role)
                .HasDatabaseName("IX_Users_Role");
        });
    }

    private void ConfigureTravelRequest(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TravelRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.UserId)
                .IsRequired();
                
            entity.Property(e => e.OriginCity)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.DestinationCity)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.DateFrom)
                .IsRequired();
                
            entity.Property(e => e.DateTo)
                .IsRequired();
                
            entity.Property(e => e.Justification)
                .IsRequired()
                .HasMaxLength(1000);
                
            entity.Property(e => e.Status)
                .IsRequired()
                .HasConversion<int>();
                
            entity.Property(e => e.CreatedAt)
                .IsRequired();
                
            entity.Property(e => e.UpdatedAt);
            
            entity.Property(e => e.RejectionReason)
                .HasMaxLength(500);
                
            entity.Property(e => e.ApprovedBy);
            
            entity.Property(e => e.ApprovedAt);

            // Relación con User
            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_TravelRequests_Users_UserId");
                
            // Relación con User (ApprovedBy)
            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.ApprovedBy)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_TravelRequests_Users_ApprovedBy");

            // Índices
            entity.HasIndex(e => e.UserId)
                .HasDatabaseName("IX_TravelRequests_UserId");
                
            entity.HasIndex(e => e.Status)
                .HasDatabaseName("IX_TravelRequests_Status");
                
            entity.HasIndex(e => e.DateFrom)
                .HasDatabaseName("IX_TravelRequests_DateFrom");
                
            entity.HasIndex(e => e.CreatedAt)
                .HasDatabaseName("IX_TravelRequests_CreatedAt");
                
            // Índice compuesto para consultas frecuentes
            entity.HasIndex(e => new { e.UserId, e.Status })
                .HasDatabaseName("IX_TravelRequests_UserId_Status");
        });
    }

    private void ConfigurePasswordResetCode(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PasswordResetCode>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.UserId)
                .IsRequired();
                
            entity.Property(e => e.Code)
                .IsRequired()
                .HasMaxLength(50);
                
            entity.Property(e => e.ExpiresAt)
                .IsRequired();
                
            entity.Property(e => e.CreatedAt)
                .IsRequired();
                
            entity.Property(e => e.IsUsed)
                .IsRequired()
                .HasDefaultValue(false);
                
            entity.Property(e => e.UsedAt);

            // Relación con User
            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_PasswordResetCodes_Users_UserId");

            // Índices
            entity.HasIndex(e => e.UserId)
                .HasDatabaseName("IX_PasswordResetCodes_UserId");
                
            entity.HasIndex(e => e.Code)
                .HasDatabaseName("IX_PasswordResetCodes_Code");
                
            entity.HasIndex(e => e.ExpiresAt)
                .HasDatabaseName("IX_PasswordResetCodes_ExpiresAt");
                
            entity.HasIndex(e => e.IsUsed)
                .HasDatabaseName("IX_PasswordResetCodes_IsUsed");
                
            // Índice compuesto para consultas de códigos válidos
            entity.HasIndex(e => new { e.Code, e.IsUsed, e.ExpiresAt })
                .HasDatabaseName("IX_PasswordResetCodes_Code_IsUsed_ExpiresAt");
        });
    }
}
