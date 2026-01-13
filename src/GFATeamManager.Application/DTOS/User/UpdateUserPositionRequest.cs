using GFATeamManager.Domain.Enums;

namespace GFATeamManager.Application.DTOS.User;

public class UpdateUserPositionRequest
{
    public PlayerUnit? Unit { get; set; }
    public PlayerPosition? Position { get; set; }
}
