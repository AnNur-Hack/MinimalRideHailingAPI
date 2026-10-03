using Microsoft.Extensions.Logging;
using MinimalRideHailingAPI.Domain.Entities;
using MinimalRideHailingAPI.Domain.Enums;
using MinimalRideHailingAPI.DTOs;
using MinimalRideHailingAPI.DTOs.Responses;
using MinimalRideHailingAPI.Repositories.Interface;
using MinimalRideHailingAPI.Services.Interfaces;

namespace MinimalRideHailingAPI.Services.Implementations;

public class AdminService(
    IUserRepository userRepository,
    IDriverProfileRepository driverProfileRepository,
    IAuditLogRepository auditLogRepository,
    ILogger<AdminService> logger) : IAdminService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IDriverProfileRepository _driverProfileRepository =
        driverProfileRepository;
    private readonly IAuditLogRepository _auditLogRepository =
        auditLogRepository;
    private readonly ILogger<AdminService> _logger = logger;

    public async Task<ApiResponse> ApproveDriverAsync(
        int adminUserId,
        int driverUserId)
    {
        try
        {
            var admin = await _userRepository
                .GetUserByIdAsync(adminUserId);

            if (admin == null)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    "Admin user not found");
            }

            if (admin.Role != UserRole.Admin)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    "Only admins can approve drivers");
            }

            if (!admin.IsActive || admin.IsDeleted)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    "Admin account is not active");
            }

            var driver = await _userRepository
                .GetUserByIdAsync(driverUserId);

            if (driver == null)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    "Driver not found");
            }

            if (driver.Role != UserRole.Driver)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    "The selected user is not a driver");
            }

            var driverProfile =
                await _driverProfileRepository
                    .GetDriverProfileByUserIdAsync(driverUserId);

            if (driverProfile == null)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    "Driver profile not found");
            }

            if (driverProfile.IsApproved)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    "Driver is already approved");
            }

            driverProfile.IsApproved = true;
            driverProfile.ApprovedBy = admin.UserId;
            driverProfile.ApprovedAt = DateTime.UtcNow;
            driverProfile.UpdatedAt = DateTime.UtcNow;

            await _driverProfileRepository
                .UpdateDriverProfileAsync(driverProfile);

            await _auditLogRepository.AddAsync(new AuditLog
            {
                UserId = adminUserId,
                Action = "ApproveDriver",
                Status = "Success",
                Description =
                    $"Driver {driver.UserId} was approved"
            });

            _logger.LogInformation(
                "Admin {AdminUserId} approved driver {DriverUserId}",
                adminUserId,
                driverUserId);

            return ApiResponse.ResponseHelper.SuccessResponse(
                null,
                "Driver approved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error approving driver {DriverUserId} by admin {AdminUserId}",
                driverUserId,
                adminUserId);

            return ApiResponse.ResponseHelper.FailureResponse(
                "An error occurred while approving the driver");
        }
    }

    public async Task<ApiResponse> UpdateUserStatusAsync(int adminUserId, int targetUserId, bool isActive)
    {
    try
    {
        var admin = await _userRepository
            .GetUserByIdAsync(adminUserId);

        if (admin == null)
        {
            return ApiResponse.ResponseHelper.FailureResponse(
                "Admin user not found");
        }

        if (admin.Role != UserRole.Admin)
        {
            return ApiResponse.ResponseHelper.FailureResponse(
                "Only admins can update user status");
        }

        if (!admin.IsActive || admin.IsDeleted)
        {
            return ApiResponse.ResponseHelper.FailureResponse(
                "Admin account is not active");
        }

        var targetUser = await _userRepository
            .GetUserByIdAsync(targetUserId);

        if (targetUser == null)
        {
            return ApiResponse.ResponseHelper.FailureResponse(
                "User not found");
        }

        if (targetUser.IsDeleted)
        {
            return ApiResponse.ResponseHelper.FailureResponse(
                "This user has been deleted");
        }

        if (targetUser.Id == admin.Id)
        {
            return ApiResponse.ResponseHelper.FailureResponse(
                "You cannot change your own account status");
        }

        targetUser.IsActive = isActive;
        targetUser.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(targetUser);

        await _auditLogRepository.AddAsync(new AuditLog
        {
            UserId = adminUserId,
            Action = isActive
                ? "ActivateUser"
                : "DeactivateUser",
            Status = "Success",
            Description =
                $"Admin changed user {targetUser.UserId} status to {(isActive ? "Active" : "Inactive")}"
        });

        _logger.LogInformation(
            "Admin {AdminUserId} changed user {TargetUserId} status to {IsActive}",
            adminUserId,
            targetUserId,
            isActive);

        return ApiResponse.ResponseHelper.SuccessResponse(
            null,
            isActive
                ? "User activated successfully"
                : "User deactivated successfully");
    }
    catch (Exception ex)
    {
        _logger.LogError(
            ex,
            "Error changing status of user {TargetUserId} by admin {AdminUserId}",
            targetUserId,
            adminUserId);

        return ApiResponse.ResponseHelper.FailureResponse(
            "An error occurred while updating user status");
    }
    
    }

    public async Task<ApiResponse> GetUsersAsync(int adminUserId)
    {
        try
        {
            var admin = await _userRepository
                .GetUserByIdAsync(adminUserId);

            if (admin == null)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    "Admin user not found");
            }

            if (admin.Role != UserRole.Admin)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    "Only admins can view users");
            }

            if (!admin.IsActive || admin.IsDeleted)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    "Admin account is not active");
            }

            var users = await _userRepository
                .GetAllUsersAsync();

            var response = users.Select(user => new UserResponse
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role.ToString(),
                IsEmailVerified = user.IsEmailVerified
            }).ToList();

            return ApiResponse.ResponseHelper.SuccessResponse(
                response,
                "Users retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error retrieving users for admin {AdminUserId}",
                adminUserId);

            return ApiResponse.ResponseHelper.FailureResponse(
                "An error occurred while retrieving users");
        }
    }

    public async Task<ApiResponse> GetDriversAsync(int adminUserId)
    {
        try
        {
            var admin = await _userRepository
                .GetUserByIdAsync(adminUserId);

            if (admin == null)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    "Admin user not found");
            }

            if (admin.Role != UserRole.Admin)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    "Only admins can view drivers");
            }

            if (!admin.IsActive || admin.IsDeleted)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    "Admin account is not active");
            }

            var drivers = await _driverProfileRepository
                .GetAllDriversAsync();

            var response = drivers.Select(driver => new DriverResponse
            {
                UserId = driver.User.UserId,
                FullName = driver.User.FullName,
                PhoneNumber = driver.User.PhoneNumber,
                LicenseNumber = driver.LicenseNumber,
                IsAvailable = driver.IsAvailable,
                IsApproved = driver.IsApproved,

                VehicleType = driver.Vehicle?.VehicleType ?? default,
                VehicleMake = driver.Vehicle?.Make,
                VehicleModel = driver.Vehicle?.Model,
                VehicleColor = driver.Vehicle?.Color,
                PlateNumber = driver.Vehicle?.PlateNumber

            }).ToList();

            return ApiResponse.ResponseHelper.SuccessResponse(
                response,
                "Drivers retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error retrieving drivers for admin {AdminUserId}",
                adminUserId);

            return ApiResponse.ResponseHelper.FailureResponse(
                "An error occurred while retrieving drivers");
        }
        
    }
}