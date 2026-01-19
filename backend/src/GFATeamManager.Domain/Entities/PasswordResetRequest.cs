namespace GFATeamManager.Domain.Entities;

public class PasswordResetRequest : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    public string? TemporaryPasswordHash { get; set; }
    public DateTime? ExpirationDate { get; set; }
    
    public Guid? ApprovedById { get; set; }
    public User? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    
    public bool IsUsed { get; set; }
    public DateTime? UsedAt { get; set; }

    public PasswordResetRequest()
    {
        IsUsed = false;
    }

    public string Approve(Guid adminId)
    {
        ApprovedById = adminId;
        ApprovedAt = DateTime.UtcNow;
        
        var temporaryPassword = GenerateTemporaryPassword();
        TemporaryPasswordHash = BCrypt.Net.BCrypt.HashPassword(temporaryPassword);
        ExpirationDate = DateTime.UtcNow.AddHours(24);
        UpdatedAt = DateTime.UtcNow;
        
        return temporaryPassword;
    }

    public void MarkAsUsed()
    {
        IsUsed = true;
        UsedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsValid()
    {
        return !string.IsNullOrEmpty(TemporaryPasswordHash)
               && ExpirationDate.HasValue 
               && ExpirationDate > DateTime.UtcNow 
               && !IsUsed;
    }
    
    private static string GenerateTemporaryPassword()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 8)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}