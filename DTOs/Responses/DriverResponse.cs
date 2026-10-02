using MinimalRideHailingAPI.Domain.Enums;

namespace MinimalRideHailingAPI.DTOs.Responses;

public class DriverResponse
{
    public string UserId { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public string LicenseNumber { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsApproved { get; set; }
    public VehicleType VehicleType { get; set; }
    public string VehicleMake { get; set; }
    public string VehicleModel { get; set; }
    public string VehicleColor { get; set; }
    public string PlateNumber { get; set; }
}