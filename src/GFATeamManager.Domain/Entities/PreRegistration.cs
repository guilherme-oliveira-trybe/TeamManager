using GFATeamManager.Domain.Enums;

namespace GFATeamManager.Domain.Entities;

public class PreRegistration : BaseEntity
{
    public string Cpf { get; set; } = string.Empty;
    public ProfileType Profile { get; set; }
    public PlayerUnit? Unit { get; set; }
    public PlayerPosition? Position { get; set; }
    public string ActivationCode { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool IsUsed { get; set; }
    public DateTime? UsedAt { get; set; }
    public Guid? UserId { get; set; }
    public User? User { get; set; }
    
    public PreRegistration()
    {
        ActivationCode = GenerateRandomCode();
        ExpirationDate = DateTime.UtcNow.AddDays(7);
        IsUsed = false;
    }
    
    public bool IsExpired() => DateTime.UtcNow > ExpirationDate;

    public bool CanBeUsed() => !IsUsed && !IsExpired();

    private static string GenerateRandomCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 8)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}