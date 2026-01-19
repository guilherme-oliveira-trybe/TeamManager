using GFATeamManager.Domain.Entities;

namespace GFATeamManager.Domain.Interfaces.Repositories;

public interface IPasswordResetRequestRepository : IBaseRepository<PasswordResetRequest>
{
    Task<IEnumerable<PasswordResetRequest>> GetPendingRequestsAsync();
    Task<PasswordResetRequest?> GetValidRequestByUserIdAsync(Guid userId);
}