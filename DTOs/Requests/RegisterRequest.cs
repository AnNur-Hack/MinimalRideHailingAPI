using MinimalRideHailingAPI.Domain.Enums;

namespace MinimalRideHailingAPI.DTOs.Requests;

public class RegisterRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public UserRole Role { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
}