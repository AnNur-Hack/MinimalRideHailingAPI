using MinimalRideHailingAPI.Domain.Entities;
using MinimalRideHailingAPI.Domain.Enums;
using MinimalRideHailingAPI.DTOs.Requests;
using MinimalRideHailingAPI.DTOs.Responses;
using MinimalRideHailingAPI.Repositories.Interface;
using MinimalRideHailingAPI.Services.Interfaces;

namespace MinimalRideHailingAPI.Services.Implementations;

public class DriverService
    (IUserRepository userRepository, IDriverProfileRepository driverProfileRepository, 
        IAuditLogRepository auditLogRepository,
        ILogger<DriverService> logger, IVehicleRepository vehicleRepository) : IDriverService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IAuditLogRepository _auditLogRepository = auditLogRepository;
    private readonly ILogger<DriverService> _logger = logger;
    private readonly IDriverProfileRepository _driverProfileRepository = driverProfileRepository;
    private readonly IVehicleRepository _vehicleRepository = vehicleRepository;
    
    public async Task<ApiResponse> CreateDriverProfileAsync(int userId, CreateDriverProfileRequest request)
    {
        try
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return ApiResponse.ResponseHelper.FailureResponse($"User with {userId} not found");
                
            }

            if (user.Role != UserRole.Driver)
            {
                return ApiResponse.ResponseHelper.FailureResponse
                    ("Only drivers can create a drivers profile");
            }
            
            var existingProfile = await _driverProfileRepository.GetDriverProfileByUserIdAsync(userId);
            if (existingProfile != null)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    $"Profile with {userId} already exists");
            }
            
            var existingLicense = await _driverProfileRepository.GetDriverProfileByLicenseNumberAsync(request.LicenseNumber);
            if (existingLicense != null)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    $"Profile with {request.LicenseNumber} already exists");
            }

            var driverProfile = new DriverProfile
            {
                UserId = userId,
                LicenseNumber = request.LicenseNumber,
            };
            
            await _driverProfileRepository.AddDriverProfileAsync(driverProfile);

            await _auditLogRepository.AddAsync(new AuditLog
            {
                UserId = userId,
                Action = "CreateDriverProfile",
                Status = "Success",
                Description = "Driver Profile Created Successfully"

            });
            _logger.LogInformation($"Driver Profile created successfully for user {userId}");
            
            return ApiResponse.ResponseHelper.SuccessResponse
                (null, "Driver Profile Created  Successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating Driver Profile for user {userId}", userId);
            return ApiResponse.ResponseHelper.FailureResponse("Error occurred while creating Driver Profile");
        }
    }

    public async Task<ApiResponse> GetDriverProfileAsync(int userId)
    {
        try
        {
            var driverProfile = await _driverProfileRepository
                .GetDriverProfileByUserIdAsync(userId);

            if (driverProfile == null)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    "Driver profile not found");
            }

            var response = new DriverResponse
            {
                UserId = driverProfile.User.UserId,
                FullName = driverProfile.User.FullName,
                PhoneNumber = driverProfile.User.PhoneNumber,

                LicenseNumber = driverProfile.LicenseNumber,
                IsAvailable = driverProfile.IsAvailable,
                IsApproved = driverProfile.IsApproved,

                VehicleType = driverProfile.Vehicle.VehicleType,
                VehicleMake = driverProfile.Vehicle.Make,
                VehicleModel = driverProfile.Vehicle.Model,
                VehicleColor = driverProfile.Vehicle.Color,
                PlateNumber = driverProfile.Vehicle.PlateNumber
            };

            return ApiResponse.ResponseHelper.SuccessResponse(
                response,
                "Driver profile retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error retrieving driver profile for user {UserId}",
                userId);

            return ApiResponse.ResponseHelper.FailureResponse(
                "An error occurred while retrieving driver profile");
        }
        
    }

    public async Task<ApiResponse> UpdateAvailabilityAsync(int userId, UpdateDriverAvailabilityRequest request)
    {
    try
    {
        var user = await _userRepository.GetUserByIdAsync(userId);

        if (user == null)
        {
            return ApiResponse.ResponseHelper.FailureResponse(
                "User not found");
        }

        if (user.Role != UserRole.Driver)
        {
            return ApiResponse.ResponseHelper.FailureResponse(
                "Only drivers can update availability");
        }

        if (!user.IsActive || user.IsDeleted)
        {
            return ApiResponse.ResponseHelper.FailureResponse(
                "Your account is not active");
        }

        var driverProfile =
            await _driverProfileRepository
                .GetDriverProfileByUserIdAsync(userId);

        if (driverProfile == null)
        {
            return ApiResponse.ResponseHelper.FailureResponse(
                "Driver profile not found");
        }

        if (!driverProfile.IsApproved)
        {
            return ApiResponse.ResponseHelper.FailureResponse(
                "Driver has not been approved");
        }

        driverProfile.IsAvailable = request.IsAvailable;
        driverProfile.UpdatedAt = DateTime.UtcNow;

        await _driverProfileRepository
            .UpdateDriverProfileAsync(driverProfile);

        await _auditLogRepository.AddAsync(new AuditLog
        {
            UserId = userId,
            Action = "UpdateDriverAvailability",
            Status = "Success",
            Description =
                $"Driver availability changed to {request.IsAvailable}"
        });

        _logger.LogInformation(
            "Driver {UserId} availability changed to {IsAvailable}",
            userId,
            request.IsAvailable);

        return ApiResponse.ResponseHelper.SuccessResponse(
            null,
            request.IsAvailable
                ? "You are now available for rides"
                : "You are now unavailable for rides");
    }
    catch (Exception ex)
    {
        _logger.LogError(
            ex,
            "Error updating availability for driver {UserId}",
            userId);

        return ApiResponse.ResponseHelper.FailureResponse(
            "An error occurred while updating availability");
    }
    
    }

    public async Task<ApiResponse> UpdateDriverProfileAsync(int userId, CreateDriverProfileRequest request)
    {

        try
        {
            var driverProfile = await _driverProfileRepository
                .GetDriverProfileByUserIdAsync(userId);

            if (driverProfile == null)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    "Driver profile not found");
            }

            if (driverProfile.LicenseNumber != request.LicenseNumber)
            {
                var existingLicense =
                    await _driverProfileRepository
                        .GetDriverProfileByLicenseNumberAsync(
                            request.LicenseNumber);

                if (existingLicense != null &&
                    existingLicense.UserId != userId)
                {
                    return ApiResponse.ResponseHelper.FailureResponse(
                        "License number already exists");
                }
            }

            driverProfile.LicenseNumber = request.LicenseNumber;
            driverProfile.UpdatedAt = DateTime.UtcNow;

            await _driverProfileRepository
                .UpdateDriverProfileAsync(driverProfile);

            await _auditLogRepository.AddAsync(new AuditLog
            {
                UserId = userId,
                Action = "UpdateDriverProfile",
                Status = "Success",
                Description = "Driver profile updated successfully"
            });

            _logger.LogInformation(
                "Driver profile updated successfully for user {UserId}",
                userId);

            return ApiResponse.ResponseHelper.SuccessResponse(
                null,
                "Driver profile updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error updating driver profile for user {UserId}",
                userId);

            return ApiResponse.ResponseHelper.FailureResponse(
                "An error occurred while updating driver profile");
        }
        
    }

    public async Task<ApiResponse> AddVehicleAsync(int userId, CreateVehicleRequest request)
    {
        try
        {
         var driverProfile = await _driverProfileRepository
            .GetDriverProfileByUserIdAsync(userId);

        if (driverProfile == null)
        {
            return ApiResponse.ResponseHelper.FailureResponse(
                "Driver profile not found");
        }

        var existingVehicle =
            await _vehicleRepository
                .GetVehicleByDriverProfileIdAsync(
                    driverProfile.Id);

        if (existingVehicle != null)
        {
            return ApiResponse.ResponseHelper.FailureResponse(
                "Driver already has a vehicle");
        }

        var existingPlateNumber =
            await _vehicleRepository
                .GetVehicleByPlateNumberAsync(
                    request.PlateNumber);

        if (existingPlateNumber != null)
        {
            return ApiResponse.ResponseHelper.FailureResponse(
                "Plate number already exists");
        }

        var vehicle = new Vehicle
        {
            DriverProfileId = driverProfile.Id,
            Make = request.Make,
            Model = request.Model,
            Color = request.Color,
            Year = request.Year,
            PlateNumber = request.PlateNumber,
            VehicleType = request.VehicleType
        };

        await _vehicleRepository.AddAsync(vehicle);

        await _auditLogRepository.AddAsync(new AuditLog
        {
            UserId = userId,
            Action = "AddVehicle",
            Status = "Success",
            Description = "Vehicle added successfully"
        });

        _logger.LogInformation(
            "Vehicle added successfully for user {UserId}",
            userId);

        return ApiResponse.ResponseHelper.SuccessResponse(
            null,
            "Vehicle added successfully");
        }catch (Exception ex)
        {
          _logger.LogError(
            ex,
            "Error adding vehicle for user {UserId}",
            userId);

        return ApiResponse.ResponseHelper.FailureResponse(
            "An error occurred while adding vehicle");
        }
    
    }
}