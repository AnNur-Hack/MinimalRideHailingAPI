using MinimalRideHailingAPI.Domain.Entities;

namespace MinimalRideHailingAPI.Repositories.Interface;

public interface IRideRepository
{
    public Task<Ride?> GetRideByRideReferenceAsync(string rideReference);
    public Task<Ride?> GetRideByIdAsync(int id);
    public Task<IEnumerable<Ride>> GetPassengerRidesAsync(int passengerId);
    public Task<IEnumerable<Ride>> GetDriverRidesAsync(int driverId);
    Task<IEnumerable<Ride>> GetAvailableRidesAsync();

    
    public Task AddRideAsync(Ride ride);
    public Task UpdateRideAsync(Ride ride);
}