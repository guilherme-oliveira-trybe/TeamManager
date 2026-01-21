using GFATeamManager.Domain.Entities;
using GFATeamManager.Domain.Interfaces.Repositories;
using GFATeamManager.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace GFATeamManager.Infrastructure.Data.Repositories;

public class PreRegistrationRepository : BaseRepository<PreRegistration>, IPreRegistrationRepository
{
    public PreRegistrationRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PreRegistration?> GetByCpfAsync(string cpf)
    {
        return await _dbSet
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Cpf == cpf);
    }

    public async Task<PreRegistration?> GetByActivationCodeAsync(string activationCode)
    {
        return await _dbSet
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.ActivationCode == activationCode);
    }

    public async Task<PreRegistration?> GetValidPreRegistrationAsync(string cpf, string activationCode)
    {
        return await _dbSet
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => 
                p.Cpf == cpf && 
                p.ActivationCode == activationCode && 
                !p.IsUsed && 
                p.ExpirationDate > DateTime.UtcNow);
    }

    public async Task<IEnumerable<PreRegistration>> GetUnusedByCpfAsync(string cpf)
    {
        return await _dbSet
            .Where(p => p.Cpf == cpf && !p.IsUsed)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PreRegistration>> GetExpiredAsync()
    {
        return await _dbSet
            .Where(p => !p.IsUsed && p.ExpirationDate < DateTime.UtcNow)
            .ToListAsync();
    }

    public async Task<bool> ActivationCodeExistsAsync(string activationCode)
    {
        return await _dbSet.AnyAsync(p => p.ActivationCode == activationCode);
    }
}