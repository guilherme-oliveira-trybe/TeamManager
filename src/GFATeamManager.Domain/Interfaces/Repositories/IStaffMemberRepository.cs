using GFATeamManager.Domain.Entities;

namespace GFATeamManager.Domain.Interfaces.Repositories;

public interface IStaffMemberRepository : IBaseRepository<StaffMember>
{
    Task<IEnumerable<StaffMember>> GetBySectorIdAsync(Guid sectorId);
    Task<bool> EmailExistsAsync(string email, Guid? excludeStaffId = null);
}
