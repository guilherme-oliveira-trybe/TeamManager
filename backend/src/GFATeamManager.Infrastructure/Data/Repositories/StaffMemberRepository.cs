using GFATeamManager.Domain.Entities;
using GFATeamManager.Domain.Interfaces.Repositories;
using GFATeamManager.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace GFATeamManager.Infrastructure.Data.Repositories;

public class StaffMemberRepository : BaseRepository<StaffMember>, IStaffMemberRepository
{
    public StaffMemberRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<StaffMember>> GetBySectorIdAsync(Guid sectorId)
    {
        return await _dbSet
            .Where(sm => sm.SectorId == sectorId)
            .OrderBy(sm => sm.FullName)
            .ToListAsync();
    }

    public async Task<bool> EmailExistsAsync(string email, Guid? excludeStaffId = null)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var query = _dbSet.Where(sm => sm.Email == email);

        if (excludeStaffId.HasValue)
            query = query.Where(sm => sm.Id != excludeStaffId.Value);

        return await query.AnyAsync();
    }
}
