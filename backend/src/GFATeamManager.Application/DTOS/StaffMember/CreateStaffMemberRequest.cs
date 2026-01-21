namespace GFATeamManager.Application.DTOS.StaffMember;

public class CreateStaffMemberRequest
{
    public Guid SectorId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Specialty { get; set; }
}
