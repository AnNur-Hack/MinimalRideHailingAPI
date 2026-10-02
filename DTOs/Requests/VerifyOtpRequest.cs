namespace MinimalRideHailingAPI.DTOs.Requests;

public class VerifyOtpRequest
{
    public string Email { get; set; }
    public string OtpCode { get; set; }
    public string Purpose { get; set; }
}