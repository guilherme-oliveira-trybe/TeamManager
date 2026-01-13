namespace GFATeamManager.Application.DTOS.Activities;

public class ActivityResponse
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public string? TargetUnit { get; set; }
    public List<ActivityItemResponse> Items { get; set; } = [];
}
