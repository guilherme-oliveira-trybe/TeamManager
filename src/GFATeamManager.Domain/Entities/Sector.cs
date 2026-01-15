namespace GFATeamManager.Domain.Entities;

public class Sector : BaseEntity
{
    public Guid DepartmentId { get; set; }
    public Department Department { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ICollection<StaffMember> StaffMembers { get; set; } = [];
}
