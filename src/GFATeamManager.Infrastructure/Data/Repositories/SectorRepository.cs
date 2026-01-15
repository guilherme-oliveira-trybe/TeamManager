using GFATeamManager.Domain.Entities;
using GFATeamManager.Domain.Interfaces.Repositories;
using GFATeamManager.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace GFATeamManager.Infrastructure.Data.Repositories;

public class SectorRepository : BaseRepository<Sector>, ISectorRepository
{
    public SectorRepository(AppDbContext context) : base(context)
    {
    }

    public override async Task<Sector?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(s => s.StaffMembers)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Sector?> GetWithStaffAsync(Guid id)
    {
        return await GetByIdAsync(id);
    }

    public async Task<IEnumerable<Sector>> GetByDepartmentIdAsync(Guid departmentId)
    {
        return await _dbSet
            .Include(s => s.StaffMembers)
            .Where(s => s.DepartmentId == departmentId)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Sector>> GetAllWithStaffAsync()
    {
        return await _dbSet
            .Include(s => s.StaffMembers)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }
}
