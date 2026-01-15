namespace GFATeamManager.Application.DTOS.Sector;

public class CreateSectorRequest
{
    public Guid DepartmentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
