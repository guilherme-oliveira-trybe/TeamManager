using GFATeamManager.Domain.Entities;

namespace GFATeamManager.Domain.Interfaces.Repositories;

public interface IDepartmentRepository : IBaseRepository<Department>
{
    Task<IEnumerable<Department>> GetAllWithSectorsAsync();
}
