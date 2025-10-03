using TravelRequests.Domain.Entities;
using TravelRequests.Domain.Enums;

namespace TravelRequests.Application.Contracts.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> ExistsByEmailAsync(string email);
    Task<IEnumerable<User>> GetByRoleAsync(UserRole role);
    Task<IEnumerable<User>> GetByRoleAsync(UserRole role, int skip, int take);
    Task<int> CountByRoleAsync(UserRole role);
    Task<User?> GetWithTravelRequestsAsync(Guid userId);
    Task<IEnumerable<User>> GetUsersWithTravelRequestsAsync(int skip = 0, int take = 10);
}
