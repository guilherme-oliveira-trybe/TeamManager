using GFATeamManager.Domain.Enums;

namespace GFATeamManager.Domain.Entities;

public class ActivityItem : BaseEntity
{
    public Guid ActivityId { get; set; }
    public Activity Activity { get; set; } = null!;
    
    public string Title { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    
    public PlayerUnit? TargetUnit { get; set; }
    public List<PlayerPosition> TargetPositions { get; set; } = [];
}
