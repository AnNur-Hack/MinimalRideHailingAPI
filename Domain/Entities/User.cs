using MinimalRideHailingAPI.Domain.Enums;

namespace MinimalRideHailingAPI.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string PasswordHash { get; set; }
    public UserRole Role { get; set; }
    public bool IsEmailVerified { get; set; }
    public bool IsPhoneNumberVerified { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    
    public ICollection<Otp> Otps { get; set; }
    public ICollection<AuditLog> AuditLogs { get; set; }
    
    public DriverProfile DriverProfile { get; set; }
    public ICollection<Ride> PassengerRides { get; set; }
    public ICollection<Ride> DriverRides { get; set; }


    public User()
    {
        Otps = new List<Otp>();
        AuditLogs = new List<AuditLog>();
        PassengerRides = new List<Ride>();
        DriverRides = new List<Ride>();
    }
}