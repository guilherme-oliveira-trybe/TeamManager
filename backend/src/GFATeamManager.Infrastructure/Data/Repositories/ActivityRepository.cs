using GFATeamManager.Domain.Entities;
using GFATeamManager.Domain.Interfaces.Repositories;
using GFATeamManager.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace GFATeamManager.Infrastructure.Data.Repositories;

public class ActivityRepository : BaseRepository<Activity>, IActivityRepository
{
    public ActivityRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Activity>> GetActivitiesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {        
        return await _dbSet
            .Include(a => a.Items)
            .Where(a => a.StartDate >= startDate && a.StartDate <= endDate)
            .OrderBy(a => a.StartDate)
            .ToListAsync();
    }

    public override async Task<Activity?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(a => a.Items)
            .FirstOrDefaultAsync(a => a.Id == id);
    }
}
