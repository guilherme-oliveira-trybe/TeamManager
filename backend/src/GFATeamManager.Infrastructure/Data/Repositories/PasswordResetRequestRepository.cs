using GFATeamManager.Domain.Entities;
using GFATeamManager.Domain.Interfaces.Repositories;
using GFATeamManager.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace GFATeamManager.Infrastructure.Data.Repositories;

public class PasswordResetRequestRepository : BaseRepository<PasswordResetRequest>, IPasswordResetRequestRepository
{
    public PasswordResetRequestRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<PasswordResetRequest>> GetPendingRequestsAsync()
    {
        return await _dbSet
            .Include(p => p.User)
            .ThenInclude(u => u.EmergencyContact)
            .Where(p => 
                p.ApprovedAt == null &&
                !p.IsDeleted)
            .OrderBy(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<PasswordResetRequest?> GetValidRequestByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => 
                p.UserId == userId &&
                p.ApprovedAt != null &&
                !p.IsUsed &&
                p.ExpirationDate > DateTime.UtcNow);
    }

    public async Task<PasswordResetRequest?> GetActiveRequestByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Include(p => p.User)
            .Where(p => 
                p.UserId == userId &&
                !p.IsDeleted &&
                (
                    p.ApprovedAt == null
                    ||
                    (p.ApprovedAt != null && 
                     !p.IsUsed && 
                     p.ExpirationDate > DateTime.UtcNow)
                ))
            .FirstOrDefaultAsync();
    }
}