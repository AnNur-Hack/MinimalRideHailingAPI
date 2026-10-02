namespace MinimalRideHailingAPI.Domain.Entities;

public class Otp
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string OtpCode { get; set; }
    public bool IsUsed { get; set; }
    public string Purpose { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public DateTime? VerifiedAt { get; set; }
    
    public User User { get; set; }
}