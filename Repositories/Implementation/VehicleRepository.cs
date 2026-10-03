using Dapper;
using MinimalRideHailingAPI.Data;
using MinimalRideHailingAPI.Domain.Entities;
using MinimalRideHailingAPI.Repositories.Interface;

namespace MinimalRideHailingAPI.Repositories.Implementation;

public class VehicleRepository(DapperContext context) : IVehicleRepository
{
    private readonly DapperContext _context = context;
    public async Task<Vehicle?> GetVehicleByDriverProfileIdAsync(int driverProfileId)
    {
        const string sql = """
                           SELECT *
                           FROM Vehicles
                           WHERE DriverProfileId = @DriverProfileId;
                           """;

        using var connection = _context.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<Vehicle>(
            sql,
            new { DriverProfileId = driverProfileId });    }

    public async Task<Vehicle?> GetVehicleByPlateNumberAsync(string plateNumber)
    {
        const string sql = """
                           SELECT *
                           FROM Vehicles
                           WHERE PlateNumber = @PlateNumber;
                           """;

        using var connection = _context.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<Vehicle>(
            sql,
            new { PlateNumber = plateNumber });    }

    public async Task AddAsync(Vehicle vehicle)
    {
        const string sql = """
                           INSERT INTO Vehicles
                           (
                               DriverProfileId,
                               Make,
                               Model,
                               Color,
                               Year,
                               PlateNumber,
                               VehicleType,
                               CreatedAt,
                               UpdatedAt
                           )
                           VALUES
                           (
                               @DriverProfileId,
                               @Make,
                               @Model,
                               @Color,
                               @Year,
                               @PlateNumber,
                               @VehicleType,
                               @CreatedAt,
                               @UpdatedAt
                           );
                           """;

        using var connection = _context.CreateConnection();

        await connection.ExecuteAsync(sql, vehicle);    }

    public async Task UpdateAsync(Vehicle vehicle)
    {
        const string sql = """
                           UPDATE Vehicles
                           SET
                               Make = @Make,
                               Model = @Model,
                               Color = @Color,
                               Year = @Year,
                               PlateNumber = @PlateNumber,
                               VehicleType = @VehicleType,
                               UpdatedAt = @UpdatedAt
                           WHERE Id = @Id;
                           """;

        using var connection = _context.CreateConnection();

        await connection.ExecuteAsync(sql, vehicle);
        }
}