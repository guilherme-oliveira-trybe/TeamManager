namespace GFATeamManager.Application.DTOS.User;

public class CompleteRegistrationRequest
{
    public string Cpf { get; set; } = string.Empty;
    public string ActivationCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public decimal Weight { get; set; }
    public int Height { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string EmergencyContactName { get; set; } = string.Empty;
    public string EmergencyContactPhone { get; set; } = string.Empty;
}