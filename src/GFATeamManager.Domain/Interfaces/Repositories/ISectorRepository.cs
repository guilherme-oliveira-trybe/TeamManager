using GFATeamManager.Domain.Entities;

namespace GFATeamManager.Domain.Interfaces.Repositories;

public interface ISectorRepository : IBaseRepository<Sector>
{
    Task<Sector?> GetWithStaffAsync(Guid id);
    Task<IEnumerable<Sector>> GetByDepartmentIdAsync(Guid departmentId);
    Task<IEnumerable<Sector>> GetAllWithStaffAsync();
}
