using MinimalRideHailingAPI.Domain.Enums;

namespace MinimalRideHailingAPI.DTOs.Requests;

public class CreateVehicleRequest
{
    public string? Make { get; set; } 
    public string? Model { get; set; } 
    public string? Color { get; set; } 
    public string? Year { get; set; } 
    public string? PlateNumber { get; set; } 
    public VehicleType VehicleType { get; set; }
}