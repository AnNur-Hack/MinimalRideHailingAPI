using Microsoft.Extensions.Logging;
using MinimalRideHailingAPI.Domain.Entities;
using MinimalRideHailingAPI.Domain.Enums;
using MinimalRideHailingAPI.DTOs;
using MinimalRideHailingAPI.DTOs.Requests;
using MinimalRideHailingAPI.DTOs.Responses;
using MinimalRideHailingAPI.Repositories.Interface;
using MinimalRideHailingAPI.Services.Interfaces;

namespace MinimalRideHailingAPI.Services.Implementations;

public class PassengerService(
    IUserRepository userRepository,
    IAuditLogRepository auditLogRepository,
    ILogger<PassengerService> logger) : IPassengerService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IAuditLogRepository _auditLogRepository = auditLogRepository;
    private readonly ILogger<PassengerService> _logger = logger;

    public async Task<ApiResponse> GetProfileAsync(int userId)
    {
        try
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    "User not found");
            }

            if (user.Role != UserRole.Passenger)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    "Only passengers can access this profile");
            }

            if (!user.IsActive || user.IsDeleted)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    "Your account is not active");
            }

            return ApiResponse.ResponseHelper.SuccessResponse(
                user,
                "Passenger profile retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error retrieving passenger profile for user {UserId}",
                userId);

            return ApiResponse.ResponseHelper.FailureResponse(
                "An error occurred while retrieving your profile");
        }
    }

    public async Task<ApiResponse> UpdateProfileAsync(
        int userId,
        UpdateProfileRequest request)
    {
        try
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    "User not found");
            }

            if (user.Role != UserRole.Passenger)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    "Only passengers can update this profile");
            }

            if (!user.IsActive || user.IsDeleted)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    "Your account is not active");
            }

            user.FullName = request.FullName;
            user.PhoneNumber = request.PhoneNumber;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            await _auditLogRepository.AddAsync(new AuditLog
            {
                UserId = userId,
                Action = "UpdatePassengerProfile",
                Status = "Success",
                Description = "Passenger profile updated successfully"
            });

            _logger.LogInformation(
                "Passenger profile updated for user {UserId}",
                userId);

            return ApiResponse.ResponseHelper.SuccessResponse(
                null,
                "Passenger profile updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error updating passenger profile for user {UserId}",
                userId);

            return ApiResponse.ResponseHelper.FailureResponse(
                "An error occurred while updating your profile");
        }
    }
}