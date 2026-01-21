using GFATeamManager.Domain.Entities;
using GFATeamManager.Domain.Interfaces.Repositories;
using GFATeamManager.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace GFATeamManager.Infrastructure.Data.Repositories;

public class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
{
    public DepartmentRepository(AppDbContext context) : base(context)
    {
    }

    public override async Task<Department?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(d => d.Sectors)
            .ThenInclude(s => s.StaffMembers)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<IEnumerable<Department>> GetAllWithSectorsAsync()
    {
        return await _dbSet
            .Include(d => d.Sectors)
            .ThenInclude(s => s.StaffMembers)
            .ToListAsync();
    }
}
