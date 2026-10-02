namespace MinimalRideHailingAPI.DTOs.Responses;

public class UserResponse
{
    public string UserId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Role { get; set; }
    public bool IsEmailVerified { get; set; }
}