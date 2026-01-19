namespace GFATeamManager.Application.DTOS.Sector;

public class UpdateSectorRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
