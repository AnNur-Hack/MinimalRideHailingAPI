using MinimalRideHailingAPI.DTOs.Requests;
using MinimalRideHailingAPI.DTOs.Responses;

namespace MinimalRideHailingAPI.Services.Interfaces;

public interface IRideService
{
    Task<ApiResponse> CreateRideAsync(
        int userId,
        CreateRideRequest request);

    Task<ApiResponse> GetRideByReferenceAsync(
        int userId,
        string rideReference);

    Task<ApiResponse> GetPassengerRidesAsync(
        int userId);

    Task<ApiResponse> GetDriverRidesAsync(
        int userId);
    Task<ApiResponse> GetAvailableRidesAsync(int userId);
    Task<ApiResponse> AcceptRideAsync(
        int userId,
        string rideReference);
    

    Task<ApiResponse> UpdateRideStatusAsync(
        int userId,
        string rideReference,
        UpdateRideStatusRequest request);

    Task<ApiResponse> CancelRideAsync(
        int userId,
        string rideReference);
}