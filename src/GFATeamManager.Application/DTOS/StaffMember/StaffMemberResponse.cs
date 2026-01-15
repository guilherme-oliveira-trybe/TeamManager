namespace GFATeamManager.Application.DTOS.StaffMember;

public class StaffMemberResponse
{
    public Guid Id { get; set; }
    public Guid SectorId { get; set; }
    public string SectorName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Specialty { get; set; }
    public string? PhotoUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}
