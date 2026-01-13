using GFATeamManager.Domain.Entities;
using GFATeamManager.Domain.Enums;

namespace GFATeamManager.Domain.Interfaces.Repositories;

public interface IActivityRepository : IBaseRepository<Activity>
{
    Task<IEnumerable<Activity>> GetActivitiesByDateRangeAsync(DateTime startDate, DateTime endDate);
}
