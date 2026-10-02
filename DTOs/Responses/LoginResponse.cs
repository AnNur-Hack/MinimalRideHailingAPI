namespace MinimalRideHailingAPI.DTOs.Responses;

public class LoginResponse
{
    public string Token { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public UserResponse User { get; set; }
}