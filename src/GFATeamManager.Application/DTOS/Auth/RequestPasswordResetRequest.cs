namespace GFATeamManager.Application.DTOS.Auth;

public class RequestPasswordResetRequest
{
    public string Cpf { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}