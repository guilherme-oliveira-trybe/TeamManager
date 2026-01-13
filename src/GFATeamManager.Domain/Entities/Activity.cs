using GFATeamManager.Domain.Enums;

namespace GFATeamManager.Domain.Entities;

public class Activity : BaseEntity
{
    public ActivityType Type { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public PlayerUnit? TargetUnit { get; set; }
    
    public Guid CreatedById { get; set; }
    public User CreatedBy { get; set; } = null!;
    
    public ICollection<ActivityItem> Items { get; set; } = new List<ActivityItem>();
}
