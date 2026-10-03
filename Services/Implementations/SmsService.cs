using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using MinimalRideHailingAPI.Configuration;
using MinimalRideHailingAPI.Domain.Entities;
using MinimalRideHailingAPI.Services.Interfaces;

namespace MinimalRideHailingAPI.Services.Implementations;

public class TelecomeAbodeSmsService(
    HttpClient httpClient,
    IOptions<TelecomeAbodeSettings> settings,
    ILogger<TelecomeAbodeSmsService> logger) : ISmsService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly TelecomeAbodeSettings _settings = settings.Value;
    private readonly ILogger<TelecomeAbodeSmsService> _logger = logger;

    public async Task SendSmsAsync(
        string recipientNumber,
        string message)
    {
        if (string.IsNullOrWhiteSpace(recipientNumber))
        {
            _logger.LogInformation(
                "SMS sending cancelled: recipient number is empty");

            return;
        }

        if (string.IsNullOrWhiteSpace(_settings.BaseUrl) ||
            string.IsNullOrWhiteSpace(_settings.ApiKey))
        {
            _logger.LogInformation(
                "SMS sending cancelled: BaseUrl or ApiKey is missing");

            return;
        }

        var rawNumbers = recipientNumber.Split(
            ',',
            StringSplitOptions.RemoveEmptyEntries);

        var normalizedNumbers = new List<string>();

        foreach (var number in rawNumbers)
        {
            var cleanPhone = number.Trim();

            if (cleanPhone.StartsWith("+234"))
            {
                cleanPhone = "0" + cleanPhone.Substring(4);
            }
            else if (cleanPhone.StartsWith("234"))
            {
                cleanPhone = "0" + cleanPhone.Substring(3);
            }
            else if (cleanPhone.StartsWith("+"))
            {
                cleanPhone = cleanPhone.Substring(1);
            }

            normalizedNumbers.Add(cleanPhone);
        }

        if (normalizedNumbers.Count == 0)
        {
            _logger.LogInformation(
                "SMS sending cancelled: no valid recipient");

            return;
        }

        var bulkPhones = string.Join(
            ",",
            normalizedNumbers);

        var payload = new Dictionary<string, string>
        {
            { "subject", _settings.Subject },
            { "bulkPhones", bulkPhones },
            { "message", message }
        };

        try
        {
            var content = new FormUrlEncodedContent(payload);

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                _settings.BaseUrl)
            {
                Content = content
            };

            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    _settings.ApiKey);

            request.Headers.Add(
                "Accept",
                "application/json");

            _logger.LogInformation(
                "Sending SMS to {PhoneNumbers}",
                bulkPhones);

            var response = await _httpClient.SendAsync(request);

            var responseContent =
                await response.Content.ReadAsStringAsync();

            _logger.LogInformation(
                "TelecomeAbode response: {Response}",
                responseContent);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation(
                    "SMS sent successfully to {PhoneNumbers}",
                    bulkPhones);
            }
            else
            {
                _logger.LogWarning(
                    "SMS failed. Status: {StatusCode}",
                    response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error sending SMS to {PhoneNumbers}",
                bulkPhones);
        }
    }
}