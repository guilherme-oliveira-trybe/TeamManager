namespace GFATeamManager.Application.DTOS.StaffMember;

public class UpdateStaffMemberRequest
{
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Specialty { get; set; }
}
