using GFATeamManager.Domain.Enums;

namespace GFATeamManager.Application.DTOS.Activities;

public class CreateActivityRequest
{
    public ActivityType Type { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public PlayerUnit? TargetUnit { get; set; }
}
