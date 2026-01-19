using GFATeamManager.Domain.Enums;

namespace GFATeamManager.Application.DTOS.PreRegistration;

public class CreatePreRegistrationRequest
{
    public string Cpf { get; set; } = string.Empty;
    public ProfileType Profile { get; set; }
    public PlayerUnit? Unit { get; set; }
    public PlayerPosition? Position { get; set; }
}