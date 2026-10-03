using MinimalRideHailingAPI.DTOs.Requests;
using MinimalRideHailingAPI.DTOs.Responses;

namespace MinimalRideHailingAPI.Services.Interfaces;

public interface IPassengerService
{
    Task<ApiResponse> GetProfileAsync(int userId);

    Task<ApiResponse> UpdateProfileAsync(
        int userId,
        UpdateProfileRequest request);
}