using MinimalRideHailingAPI.Domain.Enums;

namespace MinimalRideHailingAPI.DTOs.Requests;

public class UpdateRideStatusRequest
{
    public RideStatus Status { get; set; }

}