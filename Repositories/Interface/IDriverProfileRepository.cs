using MinimalRideHailingAPI.Domain.Entities;

namespace MinimalRideHailingAPI.Repositories.Interface;

public interface IDriverProfileRepository
{
    public Task<DriverProfile?> GetDriverProfileByUserIdAsync(int userId); 
    public Task<DriverProfile?> GetDriverProfileByLicenseNumberAsync(string licenseNumber);
    public Task<IEnumerable<DriverProfile?>> GetAvailableDriversAsync();
    Task<IEnumerable<DriverProfile>> GetAllDriversAsync();
    public Task AddDriverProfileAsync(DriverProfile profile);
    public Task UpdateDriverProfileAsync(DriverProfile profile);
}