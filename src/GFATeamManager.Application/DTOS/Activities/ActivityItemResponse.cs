namespace GFATeamManager.Application.DTOS.Activities;

public class ActivityItemResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? TargetUnit { get; set; }
    public List<string> TargetPositions { get; set; } = [];
}
