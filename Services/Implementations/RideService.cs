using MinimalRideHailingAPI.Domain.Entities;
using MinimalRideHailingAPI.Domain.Enums;
using MinimalRideHailingAPI.DTOs.Requests;
using MinimalRideHailingAPI.DTOs.Responses;
using MinimalRideHailingAPI.Helpers;
using MinimalRideHailingAPI.Repositories.Interface;
using MinimalRideHailingAPI.Services.Interfaces;

namespace MinimalRideHailingAPI.Services.Implementations;

public class RideService(IUserRepository userRepository, IRideRepository rideRepository, 
    IAuditLogRepository auditLogRepository,
    ILogger<RideService> logger, IDriverProfileRepository driverProfileRepository) : IRideService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IRideRepository _rideRepository = rideRepository;
    private readonly ILogger<RideService> _logger = logger;
    private readonly IAuditLogRepository _auditLogRepository = auditLogRepository;
    private readonly IDriverProfileRepository _driverProfileRepository = driverProfileRepository;
    
    public async Task<ApiResponse> CreateRideAsync(int userId, CreateRideRequest request)
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
                    "Only passengers can request a ride");
            }

            if (!user.IsActive || user.IsDeleted)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    "Your account is not active");
            }

            var ride = new Ride
            {
                RideReference = RideReferenceGenerator.GenerateRideReference(),
                PassengerId = userId,
                PickUpLocation = request.PickUpLocation,
                Destination = request.Destination,
                Status = RideStatus.Requested
            };

            await _rideRepository.AddRideAsync(ride);

            await _auditLogRepository.AddAsync(new AuditLog
            {
                UserId = userId,
                Action = "CreateRide",
                Status = "Success",
                Description =
                    $"Ride {ride.RideReference} requested successfully"
            });

            _logger.LogInformation(
                "Ride {RideReference} created by passenger {UserId}",
                ride.RideReference,
                userId);

            return ApiResponse.ResponseHelper.SuccessResponse(
                null,
                "Ride requested successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error creating ride for passenger {UserId}",
                userId);

            return ApiResponse.ResponseHelper.FailureResponse(
                "An error occurred while requesting the ride");
        }
        
    }

    public async Task<ApiResponse> GetRideByReferenceAsync(int userId, string rideReference)
    {
        try
        {
            var ride = await _rideRepository
                .GetRideByRideReferenceAsync(rideReference);

            if (ride == null)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    "Ride not found");
            }

            if (ride.PassengerId == userId)
            {
                return ApiResponse.ResponseHelper.SuccessResponse(
                    ride,
                    "Ride retrieved successfully");
            }

            if (ride.DriverId == userId)
            {
                return ApiResponse.ResponseHelper.SuccessResponse(
                    ride,
                    "Ride retrieved successfully");
            }

            return ApiResponse.ResponseHelper.FailureResponse(
                "You are not authorized to view this ride");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error retrieving ride {RideReference} for user {UserId}",
                rideReference,
                userId);

            return ApiResponse.ResponseHelper.FailureResponse(
                "An error occurred while retrieving the ride");
        }
        
    }

    public async Task<ApiResponse> GetPassengerRidesAsync(int userId)
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
                    "Only passengers can view passenger rides");
            }

            var rides = await _rideRepository
                .GetPassengerRidesAsync(userId);

            return ApiResponse.ResponseHelper.SuccessResponse(
                rides,
                "Passenger rides retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error retrieving rides for passenger {UserId}",
                userId);

            return ApiResponse.ResponseHelper.FailureResponse(
                "An error occurred while retrieving rides");
        }
        
    }

    public async Task<ApiResponse> GetDriverRidesAsync(int userId)
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
                    "Only drivers can view driver rides");
            }

            var rides = await _rideRepository
                .GetDriverRidesAsync(userId);

            return ApiResponse.ResponseHelper.SuccessResponse(
                rides,
                "Driver rides retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error retrieving rides for driver {UserId}",
                userId);

            return ApiResponse.ResponseHelper.FailureResponse(
                "An error occurred while retrieving rides");
        }
        
    }

    public async Task<ApiResponse> GetAvailableRidesAsync(int userId)
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
                    "Only drivers can view available rides");
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

            if (!driverProfile.IsAvailable)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    "Driver is currently unavailable");
            }

            var rides = await _rideRepository
                .GetAvailableRidesAsync();

            return ApiResponse.ResponseHelper.SuccessResponse(
                rides,
                "Available rides retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error retrieving available rides for driver {UserId}",
                userId);

            return ApiResponse.ResponseHelper.FailureResponse(
                "An error occurred while retrieving available rides");
        }
        
    }

    public async Task<ApiResponse> AcceptRideAsync(int userId, string rideReference)
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
                "Only drivers can accept rides");
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

        if (!driverProfile.IsAvailable)
        {
            return ApiResponse.ResponseHelper.FailureResponse(
                "Driver is currently unavailable");
        }

        var ride = await _rideRepository
            .GetRideByRideReferenceAsync(rideReference);

        if (ride == null)
        {
            return ApiResponse.ResponseHelper.FailureResponse(
                "Ride not found");
        }

        if (ride.Status != RideStatus.Requested)
        {
            return ApiResponse.ResponseHelper.FailureResponse(
                "This ride is no longer available for acceptance");
        }

        if (ride.DriverId != null)
        {
            return ApiResponse.ResponseHelper.FailureResponse(
                "This ride has already been assigned to a driver");
        }

        ride.DriverId = userId;
        ride.Status = RideStatus.Accepted;
        ride.UpdatedAt = DateTime.UtcNow;

        await _rideRepository.UpdateRideAsync(ride);

        await _auditLogRepository.AddAsync(new AuditLog
        {
            UserId = userId,
            Action = "AcceptRide",
            Status = "Success",
            Description =
                $"Driver accepted ride {ride.RideReference}"
        });

        _logger.LogInformation(
            "Driver {UserId} accepted ride {RideReference}",
            userId,
            rideReference);

        return ApiResponse.ResponseHelper.SuccessResponse(
            null,
            "Ride accepted successfully");
    }
    catch (Exception ex)
    {
        _logger.LogError(
            ex,
            "Error accepting ride {RideReference} by driver {UserId}",
            rideReference,
            userId);

        return ApiResponse.ResponseHelper.FailureResponse(
            "An error occurred while accepting the ride");
    }
    
    }

    public async Task<ApiResponse> UpdateRideStatusAsync(int userId, string rideReference, UpdateRideStatusRequest request)
    {
    try
    {
        var ride = await _rideRepository
            .GetRideByRideReferenceAsync(rideReference);

        if (ride == null)
        {
            return ApiResponse.ResponseHelper.FailureResponse(
                "Ride not found");
        }

        var user = await _userRepository.GetUserByIdAsync(userId);

        if (user == null)
        {
            return ApiResponse.ResponseHelper.FailureResponse(
                "User not found");
        }

        if (user.Role != UserRole.Driver)
        {
            return ApiResponse.ResponseHelper.FailureResponse(
                "Only drivers can update ride status");
        }

        if (ride.DriverId != userId)
        {
            return ApiResponse.ResponseHelper.FailureResponse(
                "You are not authorized to update this ride");
        }

        if (!IsValidStatusTransition(ride.Status, request.Status))
        {
            return ApiResponse.ResponseHelper.FailureResponse(
                $"Cannot change ride status from {ride.Status} to {request.Status}");
        }

        ride.Status = request.Status;
        ride.UpdatedAt = DateTime.UtcNow;

        if (request.Status == RideStatus.Completed)
        {
            ride.CompletedAt = DateTime.UtcNow;
        }

        await _rideRepository.UpdateRideAsync(ride);

        await _auditLogRepository.AddAsync(new AuditLog
        {
            UserId = userId,
            Action = "UpdateRideStatus",
            Status = "Success",
            Description =
                $"Ride {ride.RideReference} status changed to {ride.Status}"
        });

        _logger.LogInformation(
            "Ride {RideReference} status changed to {Status} by driver {UserId}",
            ride.RideReference,
            ride.Status,
            userId);

        return ApiResponse.ResponseHelper.SuccessResponse(
            ride,
            "Ride status updated successfully");
    }
    catch (Exception ex)
    {
        _logger.LogError(
            ex,
            "Error updating ride {RideReference} for user {UserId}",
            rideReference,
            userId);

        return ApiResponse.ResponseHelper.FailureResponse(
            "An error occurred while updating ride status");
    }
    
    }

    public async Task<ApiResponse> CancelRideAsync(int userId, string rideReference)
    {
        try
        {
            var ride = await _rideRepository
                .GetRideByRideReferenceAsync(rideReference);

            if (ride == null)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    "Ride not found");
            }

            if (ride.PassengerId != userId)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    "You are not authorized to cancel this ride");
            }

            if (ride.Status != RideStatus.Requested &&
                ride.Status != RideStatus.Accepted)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    "This ride can no longer be cancelled");
            }

            ride.Status = RideStatus.Cancelled;
            ride.CancelledAt = DateTime.UtcNow;
            ride.UpdatedAt = DateTime.UtcNow;

            await _rideRepository.UpdateRideAsync(ride);

            await _auditLogRepository.AddAsync(new AuditLog
            {
                UserId = userId,
                Action = "CancelRide",
                Status = "Success",
                Description =
                    $"Ride {ride.RideReference} was cancelled by the passenger"
            });

            _logger.LogInformation(
                "Ride {RideReference} cancelled by passenger {UserId}",
                ride.RideReference,
                userId);

            return ApiResponse.ResponseHelper.SuccessResponse(
                null,
                "Ride cancelled successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error cancelling ride {RideReference} for user {UserId}",
                rideReference,
                userId);

            return ApiResponse.ResponseHelper.FailureResponse(
                "An error occurred while cancelling the ride");
        }
        
    }
    
    private bool IsValidStatusTransition(
        RideStatus currentStatus,
        RideStatus newStatus)
    {
        return currentStatus switch
        {
            RideStatus.Requested =>
                newStatus == RideStatus.Accepted ||
                newStatus == RideStatus.Cancelled,

            RideStatus.Accepted =>
                newStatus == RideStatus.DriverArriving ||
                newStatus == RideStatus.Cancelled,

            RideStatus.DriverArriving =>
                newStatus == RideStatus.DriverArrived,

            RideStatus.DriverArrived =>
                newStatus == RideStatus.InProgress,

            RideStatus.InProgress =>
                newStatus == RideStatus.Completed,

            _ => false
        };
    }
}