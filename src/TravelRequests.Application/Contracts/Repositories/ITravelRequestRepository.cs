using TravelRequests.Domain.Entities;
using TravelRequests.Domain.Enums;

namespace TravelRequests.Application.Contracts.Repositories;

public interface ITravelRequestRepository : IRepository<TravelRequest>
{
    Task<IEnumerable<TravelRequest>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<TravelRequest>> GetByUserIdAsync(Guid userId, int skip, int take);
    Task<IEnumerable<TravelRequest>> GetByStatusAsync(TravelRequestStatus status);
    Task<IEnumerable<TravelRequest>> GetByStatusAsync(TravelRequestStatus status, int skip, int take);
    Task<IEnumerable<TravelRequest>> GetByUserAndStatusAsync(Guid userId, TravelRequestStatus status);
    Task<IEnumerable<TravelRequest>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);
    Task<IEnumerable<TravelRequest>> GetByApproverAsync(Guid approverId);
    Task<IEnumerable<TravelRequest>> GetPendingRequestsAsync();
    Task<IEnumerable<TravelRequest>> GetPendingRequestsAsync(int skip, int take);
    Task<int> CountByUserIdAsync(Guid userId);
    Task<int> CountByStatusAsync(TravelRequestStatus status);
    Task<int> CountByUserAndStatusAsync(Guid userId, TravelRequestStatus status);
    Task<TravelRequest?> GetWithUserAsync(Guid travelRequestId);
    Task<IEnumerable<TravelRequest>> GetWithUsersAsync(int skip = 0, int take = 10);
}
