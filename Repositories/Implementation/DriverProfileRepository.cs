using Dapper;
using MinimalRideHailingAPI.Data;
using MinimalRideHailingAPI.Domain.Entities;
using MinimalRideHailingAPI.Repositories.Interface;

namespace MinimalRideHailingAPI.Repositories.Implementation;

public class DriverProfileRepository(DapperContext context) : IDriverProfileRepository
{
    private readonly DapperContext _context = context;
    public async Task<DriverProfile?> GetDriverProfileByUserIdAsync(int userId)
    {
        const string sql = """
                           SELECT *
                           FROM DriverProfiles
                           WHERE UserId = @UserId;
                           """;

        using var connection = _context.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<DriverProfile>(
            sql,
            new { UserId = userId });    }

    public async Task<DriverProfile?> GetDriverProfileByLicenseNumberAsync(string licenseNumber)
    {
        const string sql = """
                           SELECT *
                           FROM DriverProfiles
                           WHERE LicenseNumber = @LicenseNumber;
                           """;

        using var connection = _context.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<DriverProfile>(
            sql,
            new { LicenseNumber = licenseNumber });    }

    public async Task<IEnumerable<DriverProfile?>> GetAvailableDriversAsync()
    {
        const string sql = """
                           SELECT *
                           FROM DriverProfiles
                           WHERE IsAvailable = 1
                             AND IsApproved = 1;
                           """;

        using var connection = _context.CreateConnection();

        return await connection.QueryAsync<DriverProfile>(sql);    }

    public async Task<IEnumerable<DriverProfile>> GetAllDriversAsync()
    {
        const string sql = """
                           SELECT *
                           FROM DriverProfiles
                           ORDER BY CreatedAt DESC;
                           """;

        using var connection = _context.CreateConnection();

        var drivers = await connection.QueryAsync<DriverProfile>(sql);

        return drivers.ToList();    }

    public async Task AddDriverProfileAsync(DriverProfile profile)
    {
        const string sql = """
                           INSERT INTO DriverProfiles
                           (
                               UserId,
                               LicenseNumber,
                               IsAvailable,
                               IsApproved,
                               ApprovedBy,
                               CreatedAt,
                               ApprovedAt,
                               UpdatedAt
                           )
                           VALUES
                           (
                               @UserId,
                               @LicenseNumber,
                               @IsAvailable,
                               @IsApproved,
                               @ApprovedBy,
                               @CreatedAt,
                               @ApprovedAt,
                               @UpdatedAt
                           );
                           """;

        using var connection = _context.CreateConnection();

        await connection.ExecuteAsync(sql, profile);
    }

    public async Task UpdateDriverProfileAsync(
        DriverProfile profile)
    {
        const string sql = """
                           UPDATE DriverProfiles
                           SET
                               LicenseNumber = @LicenseNumber,
                               IsAvailable = @IsAvailable,
                               IsApproved = @IsApproved,
                               ApprovedBy = @ApprovedBy,
                               ApprovedAt = @ApprovedAt,
                               UpdatedAt = @UpdatedAt
                           WHERE Id = @Id;
                           """;

        using var connection = _context.CreateConnection();

        await connection.ExecuteAsync(sql, profile);
        
    }

   
}