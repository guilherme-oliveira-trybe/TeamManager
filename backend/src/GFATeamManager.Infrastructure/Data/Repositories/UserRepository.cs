using GFATeamManager.Domain.Entities;
using GFATeamManager.Domain.Enums;
using GFATeamManager.Domain.Interfaces.Repositories;
using GFATeamManager.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace GFATeamManager.Infrastructure.Data.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByCpfAsync(string cpf)
    {
        return await _dbSet
            .Include(u => u.EmergencyContact)
            .Include(u => u.PreRegistration)
            .FirstOrDefaultAsync(u => u.Cpf == cpf);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .Include(u => u.EmergencyContact)
            .Include(u => u.PreRegistration)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IEnumerable<User>> GetByStatusAsync(UserStatus status)
    {
        return await _dbSet
            .Include(u => u.EmergencyContact)
            .Where(u => u.Status == status)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetByProfileAsync(ProfileType profile)
    {
        return await _dbSet
            .Include(u => u.EmergencyContact)
            .Where(u => u.Profile == profile)
            .ToListAsync();
    }

    public async Task<bool> CpfExistsAsync(string cpf)
    {
        return await _dbSet.AnyAsync(u => u.Cpf == cpf);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _dbSet.AnyAsync(u => u.Email == email);
    }

    public override async Task<User?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(u => u.EmergencyContact)
            .Include(u => u.PreRegistration)
            .FirstOrDefaultAsync(u => u.Id == id);
    }
}