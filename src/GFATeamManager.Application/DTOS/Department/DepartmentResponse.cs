namespace GFATeamManager.Application.DTOS.Department;

public class DepartmentResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SectorsCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
