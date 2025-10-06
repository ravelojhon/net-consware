using Microsoft.EntityFrameworkCore;
using TravelRequests.Application.Contracts.Repositories;
using TravelRequests.Domain.Entities;
using TravelRequests.Infrastructure.Persistence;

namespace TravelRequests.Infrastructure.Repositories;

public class PasswordResetCodeRepository : Repository<PasswordResetCode>, IPasswordResetCodeRepository
{
    public PasswordResetCodeRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PasswordResetCode?> GetByCodeAsync(string code)
    {
        return await _dbSet
            .FirstOrDefaultAsync(prc => prc.Code == code);
    }

    public async Task<IEnumerable<PasswordResetCode>> GetByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Where(prc => prc.UserId == userId)
            .OrderByDescending(prc => prc.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PasswordResetCode>> GetValidCodesByUserIdAsync(Guid userId)
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Where(prc => prc.UserId == userId &&
                         !prc.IsUsed &&
                         prc.ExpiresAt > now)
            .OrderByDescending(prc => prc.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PasswordResetCode>> GetExpiredCodesAsync()
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Where(prc => prc.ExpiresAt <= now)
            .ToListAsync();
    }

    public async Task<bool> ExistsValidCodeForUserAsync(Guid userId)
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .AnyAsync(prc => prc.UserId == userId &&
                            !prc.IsUsed &&
                            prc.ExpiresAt > now);
    }

    public async Task<int> CountValidCodesForUserAsync(Guid userId)
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .CountAsync(prc => prc.UserId == userId &&
                              !prc.IsUsed &&
                              prc.ExpiresAt > now);
    }

    public async Task DeleteExpiredCodesAsync()
    {
        var now = DateTime.UtcNow;
        var expiredCodes = await _dbSet
            .Where(prc => prc.ExpiresAt <= now)
            .ToListAsync();

        if (expiredCodes.Any())
        {
            _dbSet.RemoveRange(expiredCodes);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteCodesForUserAsync(Guid userId)
    {
        var userCodes = await _dbSet
            .Where(prc => prc.UserId == userId)
            .ToListAsync();

        if (userCodes.Any())
        {
            _dbSet.RemoveRange(userCodes);
            await _context.SaveChangesAsync();
        }
    }
}
