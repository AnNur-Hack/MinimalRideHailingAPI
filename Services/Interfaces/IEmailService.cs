namespace MinimalRideHailingAPI.Services.Interfaces;

public interface IEmailService
{
    Task SendOtpEmailAsync(string recipientEmail, string fullName, string otpCode );
    Task SendWelcomeEmailAsync(
        string recipientEmail,
        string fullName);

    Task SendPasswordResetEmailAsync(
        string recipientEmail,
        string fullName,
        string otpCode);
}