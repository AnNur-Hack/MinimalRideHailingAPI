using Dapper;
using MinimalRideHailingAPI.Data;
using MinimalRideHailingAPI.Domain.Entities;
using MinimalRideHailingAPI.Domain.Enums;
using MinimalRideHailingAPI.Repositories.Interface;

namespace MinimalRideHailingAPI.Repositories.Implementation;

public class RideRepository(DapperContext context) : IRideRepository
{
    private readonly DapperContext _context = context;
    
    public async Task<Ride?> GetRideByRideReferenceAsync(string rideReference)
    {
        const string sql = """
                           SELECT *
                           FROM Rides
                           WHERE RideReference = @RideReference;
                           """;

        using var connection = _context.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<Ride>(
            sql,
            new { RideReference = rideReference });
    }

    public async Task<Ride?> GetRideByIdAsync(int id)
    {
        const string sql = """
                           SELECT *
                           FROM Rides
                           WHERE Id = @id;
                           """;
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Ride>(sql, new { Id = id });
    }

    public async Task<IEnumerable<Ride>> GetPassengerRidesAsync(int passengerId)
    {
        const string sql = """
                           SELECT *
                           FROM Rides
                           WHERE PassengerId = @PassengerId;
                            ORDER BY CreatedAt DESC;
                           """;
        using var connection = _context.CreateConnection();
        
        var rides = await  connection.QueryAsync<Ride>(sql, new { PassengerId = passengerId });
        return rides.ToList();
    }

    public async Task<IEnumerable<Ride>> GetDriverRidesAsync(int driverId)
    {
        const string sql = """
                           SELECT *
                           FROM Rides
                           WHERE DriverId = @DriverId;
                            ORDER BY CreatedAt DESC;
                           """;
        using var connection = _context.CreateConnection();
        
        var rides = await connection.QueryAsync<Ride>(sql, new { DriverId = driverId });
        return rides.ToList();
    }

    public async Task<IEnumerable<Ride>> GetAvailableRidesAsync()
    {
        const string sql = """
                           SELECT *
                           FROM Rides
                           WHERE Status = @Status;
                            AND DriverId IS NULL
                           ORDER BY CreatedAt ASC;
                           """;
        
        using var connection = _context.CreateConnection();
        
        var rides = await  connection.QueryAsync<Ride>(sql, new  { Status = RideStatus.Requested });
        return rides.ToList();
        
        
    }

    public async Task AddRideAsync(Ride ride)
    {
        const string sql = """
                           INSERT INTO Rides
                           (
                               RideReference,
                               PassengerId,
                               DriverId,
                               Fare,
                               PickUpLocation,
                               Destination,
                               Status,
                               CreatedAt,
                               UpdatedAt,
                               CompletedAt,
                               CancelledAt
                           )
                           VALUES
                           (
                               @RideReference,
                               @PassengerId,
                               @DriverId,
                               @Fare,
                               @PickUpLocation,
                               @Destination,
                               @Status,
                               @CreatedAt,
                               @UpdatedAt,
                               @CompletedAt,
                               @CancelledAt
                           );
                           """;

        using var connection = _context.CreateConnection();

        await connection.ExecuteAsync(sql, ride);
    }

    public async Task UpdateRideAsync(Ride ride)
    {
        const string sql = """
                           UPDATE Rides
                           SET
                               DriverId = @DriverId,
                               Fare = @Fare,
                               PickUpLocation = @PickUpLocation,
                               Destination = @Destination,
                               Status = @Status,
                               UpdatedAt = @UpdatedAt,
                               CompletedAt = @CompletedAt,
                               CancelledAt = @CancelledAt
                           WHERE Id = @Id;
                           """;

        using var connection = _context.CreateConnection();

        await connection.ExecuteAsync(sql, ride);
    }
}