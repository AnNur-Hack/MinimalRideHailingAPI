using MinimalRideHailingAPI.Domain.Enums;

namespace MinimalRideHailingAPI.Domain.Entities;

public class Vehicle
{public int Id { get; set; }
    public int DriverProfileId { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public string Color { get; set; }
    public string Year { get; set; }
    public string PlateNumber { get; set; }
    public VehicleType VehicleType { get; set; }
    public DriverProfile DriverProfile { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
}