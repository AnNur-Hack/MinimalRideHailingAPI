namespace MinimalRideHailingAPI.DTOs.Responses;

public class RideResponse
{
    public string RideReference { get; set; }
    public string PickUpLocation { get; set; }
    public string Destination { get; set; }
    public decimal? Fare { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
}