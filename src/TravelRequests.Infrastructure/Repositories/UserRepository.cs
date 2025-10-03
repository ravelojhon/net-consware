using Microsoft.EntityFrameworkCore;
using TravelRequests.Application.Contracts.Repositories;
using TravelRequests.Domain.Entities;
using TravelRequests.Domain.Enums;
using TravelRequests.Infrastructure.Persistence;

namespace TravelRequests.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _dbSet
            .AnyAsync(u => u.Email == email);
    }

    public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role)
    {
        return await _dbSet
            .Where(u => u.Role == role)
            .OrderBy(u => u.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role, int skip, int take)
    {
        return await _dbSet
            .Where(u => u.Role == role)
            .OrderBy(u => u.Name)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<int> CountByRoleAsync(UserRole role)
    {
        return await _dbSet
            .CountAsync(u => u.Role == role);
    }

    public async Task<User?> GetWithTravelRequestsAsync(Guid userId)
    {
        return await _dbSet
            .Include(u => _context.TravelRequests.Where(tr => tr.UserId == userId))
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<IEnumerable<User>> GetUsersWithTravelRequestsAsync(int skip = 0, int take = 10)
    {
        return await _dbSet
            .Include(u => _context.TravelRequests.Where(tr => tr.UserId == u.Id))
            .OrderBy(u => u.Name)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }
}
