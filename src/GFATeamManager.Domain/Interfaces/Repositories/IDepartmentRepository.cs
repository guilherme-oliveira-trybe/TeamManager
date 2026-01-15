using GFATeamManager.Domain.Entities;

namespace GFATeamManager.Domain.Interfaces.Repositories;

public interface IDepartmentRepository : IBaseRepository<Department>
{
    Task<Department?> GetWithSectorsAsync(Guid id);
    Task<IEnumerable<Department>> GetAllWithSectorsAsync();
}
