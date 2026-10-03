using FluentEmail.Core;
using MinimalRideHailingAPI.Services.Interfaces;
using MinimalRideHailingAPI.Templates;

namespace MinimalRideHailingAPI.Services.Implementations;

public class EmailService(
    IFluentEmail fluentEmail,
    ILogger<EmailService> logger) : IEmailService
{
    private readonly IFluentEmail _fluentEmail = fluentEmail;
    private readonly ILogger<EmailService> _logger = logger;



    public async Task SendOtpEmailAsync(
        string recipientEmail,
        string fullName,
        string otpCode)
    {
        if (string.IsNullOrWhiteSpace(recipientEmail))
        {
            _logger.LogWarning(
                "OTP email was not sent because recipient email is empty.");

            return;
        }

        try
        {
            var model = new OtpEmailModel
            {
                OtpCode = otpCode,
                FullName = fullName
            };

            var response = await _fluentEmail
                .To(recipientEmail)
                .Subject("Ride Hailing API - Email Verification")
                .UsingTemplateFromFile(
                    "Templates/OtpEmail.cshtml",
                    model)
                .SendAsync();

            if (response.Successful)
            {
                _logger.LogInformation(
                    "OTP email sent successfully to {Email}",
                    recipientEmail);
            }
            else
            {
                _logger.LogError(
                    "Failed to send OTP email to {Email}",
                    recipientEmail);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An error occurred while sending OTP email to {Email}",
                recipientEmail);
        }
    }

    

    public async Task SendWelcomeEmailAsync(
        string recipientEmail,
        string fullName)
    {
        if (string.IsNullOrWhiteSpace(recipientEmail))
        {
            _logger.LogWarning(
                "Welcome email was not sent because recipient email is empty.");

            return;
        }

        try
        {
            var model = new WelcomeEmailModel
            {
                FullName = fullName
            };

            var response = await _fluentEmail
                .To(recipientEmail)
                .Subject("Welcome to Ride Hailing API")
                .UsingTemplateFromFile(
                    "Templates/WelcomeEmail.cshtml",
                    model)
                .SendAsync();

            if (response.Successful)
            {
                _logger.LogInformation(
                    "Welcome email sent successfully to {Email}",
                    recipientEmail);
            }
            else
            {
                _logger.LogError(
                    "Failed to send welcome email to {Email}",
                    recipientEmail);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An error occurred while sending welcome email to {Email}",
                recipientEmail);
        }
    }

    

    public async Task SendPasswordResetEmailAsync(
        string recipientEmail,
        string fullName,
        string otpCode)
    {
        if (string.IsNullOrWhiteSpace(recipientEmail))
        {
            _logger.LogWarning(
                "Password reset email was not sent because recipient email is empty.");

            return;
        }

        try
        {
            var model = new PasswordResetEmailModel
            {
                FullName = fullName,
                OtpCode = otpCode
            };

            var response = await _fluentEmail
                .To(recipientEmail)
                .Subject("Ride Hailing API - Password Reset")
                .UsingTemplateFromFile(
                    "Templates/PasswordResetEmail.cshtml",
                    model)
                .SendAsync();

            if (response.Successful)
            {
                _logger.LogInformation(
                    "Password reset email sent successfully to {Email}",
                    recipientEmail);
            }
            else
            {
                _logger.LogError(
                    "Failed to send password reset email to {Email}",
                    recipientEmail);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An error occurred while sending password reset email to {Email}",
                recipientEmail);
        }
    }
}

