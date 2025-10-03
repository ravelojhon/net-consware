using TravelRequests.Domain.Entities;

namespace TravelRequests.Application.Contracts.Repositories;

public interface IPasswordResetCodeRepository : IRepository<PasswordResetCode>
{
    Task<PasswordResetCode?> GetByCodeAsync(string code);
    Task<IEnumerable<PasswordResetCode>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<PasswordResetCode>> GetValidCodesByUserIdAsync(Guid userId);
    Task<IEnumerable<PasswordResetCode>> GetExpiredCodesAsync();
    Task<bool> ExistsValidCodeForUserAsync(Guid userId);
    Task<int> CountValidCodesForUserAsync(Guid userId);
    Task DeleteExpiredCodesAsync();
    Task DeleteCodesForUserAsync(Guid userId);
}
