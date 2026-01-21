using GFATeamManager.Domain.Entities;
using GFATeamManager.Domain.Enums;

namespace GFATeamManager.Domain.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByCpfAsync(string cpf);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetByStatusAsync(UserStatus status);
    Task<IEnumerable<User>> GetByProfileAsync(ProfileType profile);
    Task<bool> CpfExistsAsync(string cpf);
    Task<bool> EmailExistsAsync(string email);
}