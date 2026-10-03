using MinimalRideHailingAPI.DTOs.Responses;

namespace MinimalRideHailingAPI.Services.Interfaces;

public interface IAdminService
{
    Task<ApiResponse> ApproveDriverAsync(
        int adminUserId,
        int driverUserId);
    Task<ApiResponse> UpdateUserStatusAsync(
        int adminUserId,
        int targetUserId,
        bool isActive);
    Task<ApiResponse> GetUsersAsync(
        int adminUserId);
    Task<ApiResponse> GetDriversAsync(
        int adminUserId);
}