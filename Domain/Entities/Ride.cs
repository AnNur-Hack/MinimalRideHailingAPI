using MinimalRideHailingAPI.Domain.Enums;

namespace MinimalRideHailingAPI.Domain.Entities;

public class Ride
{
    public int Id { get; set; }
    public string RideReference { get; set; }
    public int PassengerId { get; set; }    
    public int? DriverId { get; set; }
    public decimal? Fare { get; set; }
    public string PickUpLocation { get; set; }
    public string Destination  { get; set; }
    public RideStatus Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    
    public User Driver { get; set; }
    public User Passenger { get; set; }
}