namespace GFATeamManager.Application.DTOs.Auth;

public class PasswordResetRequestResponse
{
    public string UserFullName { get; set; } = string.Empty;
    public string? ApprovedByName { get; set; }
    public string? TemporaryPassword { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public bool IsUsed { get; set; }
}