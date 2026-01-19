namespace GFATeamManager.Application.DTOS.User;

public class UpdateUserRequest
{
    public string FullName { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public decimal Weight { get; set; }
    public int Height { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
}