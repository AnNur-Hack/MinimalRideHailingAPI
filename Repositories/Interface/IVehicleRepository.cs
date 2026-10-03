using MinimalRideHailingAPI.Domain.Entities;

namespace MinimalRideHailingAPI.Repositories.Interface;

public interface IVehicleRepository
{
    Task<Vehicle?> GetVehicleByDriverProfileIdAsync(
        int driverProfileId);

    Task<Vehicle?> GetVehicleByPlateNumberAsync(
        string plateNumber);

    Task AddAsync(Vehicle vehicle);

    Task UpdateAsync(Vehicle vehicle);
}