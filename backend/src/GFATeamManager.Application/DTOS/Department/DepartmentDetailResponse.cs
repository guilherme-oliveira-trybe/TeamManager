using GFATeamManager.Application.DTOS.Sector;

namespace GFATeamManager.Application.DTOS.Department;

public class DepartmentDetailResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<SectorResponse> Sectors { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}
