namespace MinimalRideHailingAPI.Services.Interfaces;

public interface ISmsService
{
    Task SendSmsAsync(
        string recipientNumber,
        string message);
}