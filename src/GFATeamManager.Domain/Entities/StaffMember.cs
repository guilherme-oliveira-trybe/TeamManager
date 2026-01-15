namespace GFATeamManager.Domain.Entities;

public class StaffMember : BaseEntity
{
    public Guid SectorId { get; set; }
    public Sector Sector { get; set; } = null!;
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Specialty { get; set; }
    public string? PhotoUrl { get; set; }
}
