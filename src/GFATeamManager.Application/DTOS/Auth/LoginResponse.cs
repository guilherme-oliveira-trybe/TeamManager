using GFATeamManager.Application.DTOS.User;

namespace GFATeamManager.Application.DTOS.Auth;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}