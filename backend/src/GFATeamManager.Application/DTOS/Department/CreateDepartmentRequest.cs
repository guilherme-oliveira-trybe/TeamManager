namespace GFATeamManager.Application.DTOS.Department;

public class CreateDepartmentRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
