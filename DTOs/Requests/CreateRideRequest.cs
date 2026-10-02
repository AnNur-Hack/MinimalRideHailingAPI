namespace MinimalRideHailingAPI.DTOs.Requests;

public class CreateRideRequest
{
    public string PickUpLocation { get; set; }
    public string Destination { get; set; }
}