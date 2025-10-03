using Microsoft.EntityFrameworkCore;
using TravelRequests.Application.Contracts.Repositories;
using TravelRequests.Domain.Entities;
using TravelRequests.Domain.Enums;
using TravelRequests.Infrastructure.Persistence;

namespace TravelRequests.Infrastructure.Repositories;

public class TravelRequestRepository : Repository<TravelRequest>, ITravelRequestRepository
{
    public TravelRequestRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TravelRequest>> GetByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Where(tr => tr.UserId == userId)
            .OrderByDescending(tr => tr.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TravelRequest>> GetByUserIdAsync(Guid userId, int skip, int take)
    {
        return await _dbSet
            .Where(tr => tr.UserId == userId)
            .OrderByDescending(tr => tr.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<IEnumerable<TravelRequest>> GetByStatusAsync(TravelRequestStatus status)
    {
        return await _dbSet
            .Where(tr => tr.Status == status)
            .OrderByDescending(tr => tr.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TravelRequest>> GetByStatusAsync(TravelRequestStatus status, int skip, int take)
    {
        return await _dbSet
            .Where(tr => tr.Status == status)
            .OrderByDescending(tr => tr.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<IEnumerable<TravelRequest>> GetByUserAndStatusAsync(Guid userId, TravelRequestStatus status)
    {
        return await _dbSet
            .Where(tr => tr.UserId == userId && tr.Status == status)
            .OrderByDescending(tr => tr.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TravelRequest>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
    {
        return await _dbSet
            .Where(tr => tr.DateFrom >= fromDate && tr.DateTo <= toDate)
            .OrderBy(tr => tr.DateFrom)
            .ToListAsync();
    }

    public async Task<IEnumerable<TravelRequest>> GetByApproverAsync(Guid approverId)
    {
        return await _dbSet
            .Where(tr => tr.ApprovedBy == approverId)
            .OrderByDescending(tr => tr.ApprovedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TravelRequest>> GetPendingRequestsAsync()
    {
        return await GetByStatusAsync(TravelRequestStatus.Pending);
    }

    public async Task<IEnumerable<TravelRequest>> GetPendingRequestsAsync(int skip, int take)
    {
        return await GetByStatusAsync(TravelRequestStatus.Pending, skip, take);
    }

    public async Task<int> CountByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .CountAsync(tr => tr.UserId == userId);
    }

    public async Task<int> CountByStatusAsync(TravelRequestStatus status)
    {
        return await _dbSet
            .CountAsync(tr => tr.Status == status);
    }

    public async Task<int> CountByUserAndStatusAsync(Guid userId, TravelRequestStatus status)
    {
        return await _dbSet
            .CountAsync(tr => tr.UserId == userId && tr.Status == status);
    }

    public async Task<TravelRequest?> GetWithUserAsync(Guid travelRequestId)
    {
        return await _dbSet
            .Include(tr => tr.User)
            .Include(tr => tr.ApprovedByUser)
            .FirstOrDefaultAsync(tr => tr.Id == travelRequestId);
    }

    public async Task<IEnumerable<TravelRequest>> GetWithUsersAsync(int skip = 0, int take = 10)
    {
        return await _dbSet
            .Include(tr => tr.User)
            .Include(tr => tr.ApprovedByUser)
            .OrderByDescending(tr => tr.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }
}
