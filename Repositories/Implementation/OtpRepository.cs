using Dapper;
using MinimalRideHailingAPI.Data;
using MinimalRideHailingAPI.Domain.Entities;
using MinimalRideHailingAPI.Repositories.Interface;

namespace MinimalRideHailingAPI.Repositories.Implementation;

public class OtpRepository(DapperContext context) : IOtpRepository
{
    private readonly DapperContext _context = context;
    public async Task AddAsync(Otp otp)
    {
        const string sql = """
                           INSERT INTO Otps
                           (
                               UserId,
                               OtpCode,
                               IsUsed,
                               Purpose,
                               CreatedAt,
                               ExpiresAt,
                               VerifiedAt
                           )
                           VALUES
                           (
                               @UserId,
                               @OtpCode,
                               @IsUsed,
                               @Purpose,
                               @CreatedAt,
                               @ExpiresAt,
                               @VerifiedAt
                           );
                           """;

        using var connection = _context.CreateConnection();

        await connection.ExecuteAsync(sql, otp);
        
    }

    public async Task<Otp?> GetLatestOtpAsync(int userId, string purpose)
    {
        const string sql = """
                           SELECT TOP 1 *
                           FROM Otps
                           WHERE UserId = @UserId
                             AND Purpose = @Purpose
                           ORDER BY CreatedAt DESC;
                           """;

        using var connection = _context.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<Otp>(
            sql,
            new
            {
                UserId = userId,
                Purpose = purpose
            });    }

    public async Task<Otp?> GetValidOtpAsync(int userId, string otp, string purpose)
    {
        const string sql = """
                           SELECT *
                           FROM Otps
                           WHERE UserId = @UserId
                             AND OtpCode = @OtpCode
                             AND Purpose = @Purpose
                             AND IsUsed = 0
                             AND ExpiresAt > GETUTCDATE();
                           """;

        using var connection = _context.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<Otp>(
            sql,
            new
            {
                UserId = userId,
                OtpCode = otp,
                Purpose = purpose
            });    }

    public async Task InvalidatePreviousOtpsAsync(int userId, string purpose)
    {
        const string sql = """
                           UPDATE Otps
                           SET IsUsed = 1
                           WHERE UserId = @UserId
                             AND Purpose = @Purpose
                             AND IsUsed = 0;
                           """;

        using var connection = _context.CreateConnection();

        await connection.ExecuteAsync(
            sql,
            new
            {
                UserId = userId,
                Purpose = purpose
            });
        
    }

    public async Task UpdateAsync(Otp otp)
    {
        const string sql = """
                           UPDATE Otps
                           SET
                               IsUsed = @IsUsed,
                               VerifiedAt = @VerifiedAt
                           WHERE Id = @Id;
                           """;

        using var connection = _context.CreateConnection();

        await connection.ExecuteAsync(sql, otp);
        
    }
}