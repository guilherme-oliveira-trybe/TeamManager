using GFATeamManager.Domain.Enums;

namespace GFATeamManager.Application.DTOS.Activities;

public class UpdateActivityItemRequest
{
    public string Title { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public PlayerUnit? TargetUnit { get; set; }
    public List<PlayerPosition> TargetPositions { get; set; } = [];
}
