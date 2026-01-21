using GFATeamManager.Domain.Enums;

namespace GFATeamManager.Domain.Entities;

public class User : BaseEntity
{
    public string Cpf { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public ProfileType Profile { get; set; }
    public UserStatus Status { get; set; }
    public PlayerUnit? Unit { get; set; }
    public PlayerPosition? Position { get; set; }

    public string FullName { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public decimal Weight { get; set; }
    public int Height { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public EmergencyContact? EmergencyContact { get; set; }
    public Guid PreRegistrationId { get; set; }
    public PreRegistration PreRegistration { get; set; } = null!;

    public DateTime? ActivatedAt { get; set; }
    public Guid? ActivatedById { get; set; }
    public bool RequiresPasswordChange { get; set; }

    public User()
    {
        Status = UserStatus.PendingRegistration;
    }

    public void Activate(Guid adminId)
    {
        Status = UserStatus.Active;
        ActivatedAt = DateTime.UtcNow;
        ActivatedById = adminId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject()
    {
        Status = UserStatus.Rejected;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        Status = UserStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsActive() => Status == UserStatus.Active;
    
    public void RequirePasswordChange()
    {
        RequiresPasswordChange = true;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void PasswordChanged()
    {
        RequiresPasswordChange = false;
        UpdatedAt = DateTime.UtcNow;
    }
}