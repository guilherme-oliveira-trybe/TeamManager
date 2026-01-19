namespace GFATeamManager.Domain.Entities;

public class Department : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ICollection<Sector> Sectors { get; set; } = [];
}
