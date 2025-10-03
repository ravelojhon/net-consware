using TravelRequests.Application.DTOs.TravelRequest;

namespace TravelRequests.Application.Contracts.Services;

public interface ITravelRequestService
{
    Task<TravelRequestResponse> CreateAsync(CreateTravelRequestRequest request, Guid userId);
    Task<TravelRequestResponse?> GetByIdAsync(Guid id, Guid userId, bool isApprover = false);
    Task<IEnumerable<TravelRequestListResponse>> GetByUserIdAsync(Guid userId, int skip = 0, int take = 10);
    Task<IEnumerable<TravelRequestListResponse>> GetAllAsync(int skip = 0, int take = 10);
    Task<TravelRequestResponse> UpdateAsync(Guid id, UpdateTravelRequestRequest request, Guid userId, bool isApprover = false);
    Task<TravelRequestResponse> ChangeStatusAsync(Guid id, ChangeStatusRequest request, Guid approverId);
    Task<bool> DeleteAsync(Guid id, Guid userId, bool isApprover = false);
}
