using GFATeamManager.Domain.Enums;

namespace GFATeamManager.Application.DTOS.PreRegistration;

public class PreRegistrationResponse
{
    public Guid Id { get; set; }
    public string Cpf { get; set; } = string.Empty;
    public string ActivationCode { get; set; } = string.Empty;
    public ProfileType Profile { get; set; }
    public string? Unit { get; set; }
    public string? Position { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool IsUsed { get; set; }
    public DateTime? UsedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}