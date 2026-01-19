using GFATeamManager.Domain.Entities;

namespace GFATeamManager.Application.Services.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
    Guid? ValidateToken(string token);
}