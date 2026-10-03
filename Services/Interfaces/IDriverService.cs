using MinimalRideHailingAPI.DTOs.Requests;
using MinimalRideHailingAPI.DTOs.Responses;

namespace MinimalRideHailingAPI.Services.Interfaces;

public interface IDriverService
{
    Task<ApiResponse> CreateDriverProfileAsync(int userId, CreateDriverProfileRequest request);
    Task<ApiResponse> GetDriverProfileAsync(int userId);
    Task<ApiResponse> UpdateAvailabilityAsync(
        int userId,
        UpdateDriverAvailabilityRequest request);
    Task<ApiResponse> UpdateDriverProfileAsync(int userId, CreateDriverProfileRequest request);
    Task<ApiResponse> AddVehicleAsync(
        int userId,
        CreateVehicleRequest request);
}