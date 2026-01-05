using GFATeamManager.Domain.Entities;

namespace GFATeamManager.Domain.Interfaces.Repositories;

public interface IPreRegistrationRepository : IBaseRepository<PreRegistration>
{
    Task<PreRegistration?> GetByCpfAsync(string cpf);
    Task<PreRegistration?> GetByActivationCodeAsync(string activationCode);
    Task<PreRegistration?> GetValidPreRegistrationAsync(string cpf, string activationCode);
    Task<IEnumerable<PreRegistration>> GetUnusedByCpfAsync(string cpf);
    Task<IEnumerable<PreRegistration>> GetExpiredAsync();
    Task<bool> ActivationCodeExistsAsync(string activationCode);
}