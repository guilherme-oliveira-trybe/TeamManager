using GFATeamManager.Domain.Enums;

namespace GFATeamManager.Application.DTOS.User;

public class UserResponse
{
    public Guid Id { get; set; }
    public string Cpf { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public decimal Weight { get; set; }
    public int Height { get; set; }
    public ProfileType Profile { get; set; }
    public UserStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ActivatedAt { get; set; }
    
    public EmergencyContactResponse? EmergencyContact { get; set; }
    public bool RequiresPasswordChange { get; set; }
}