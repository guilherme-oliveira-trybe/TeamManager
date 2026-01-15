using GFATeamManager.Application.DTOS.StaffMember;

namespace GFATeamManager.Application.DTOS.Sector;

public class SectorResponse
{
    public Guid Id { get; set; }
    public Guid DepartmentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int StaffMembersCount { get; set; }
    public List<StaffMemberResponse> StaffMembers { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}
