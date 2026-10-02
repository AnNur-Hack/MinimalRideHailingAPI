namespace MinimalRideHailingAPI.Domain.Entities;

public class DriverProfile
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string LicenseNumber { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsApproved { get; set; }
    public string ApprovedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ApprovedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public User User { get; set; }
    public Vehicle Vehicle { get; set; }
}