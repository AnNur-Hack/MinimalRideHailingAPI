using Dapper;
using MinimalRideHailingAPI.Data;
using MinimalRideHailingAPI.Domain.Entities;
using MinimalRideHailingAPI.Repositories.Interface;

namespace MinimalRideHailingAPI.Repositories.Implementation;

public class UserRepository(DapperContext context) : IUserRepository
{
    private readonly DapperContext _context = context;
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        const string sql = """
                           SELECT *
                           FROM Users
                           WHERE Email = @Email
                             AND IsDeleted = 0;
                           """;

        using var connection = _context.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<User>(
            sql,
            new { Email = email });    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        const string sql = """
                           SELECT *
                           FROM USERS
                           WHERE Id = @Id
                           AND IsDeleted = 0;
                           """;
        using var connection = _context.CreateConnection();
        
        return await connection.QueryFirstOrDefaultAsync<User>(
            sql,
            new { Id = id });
        
    }

    public async Task<User?> GetUserByUserIdAsync(string userId)
    {
        const string sql = """
                           SELECT *
                           FROM USERS
                           WHERE UserId = @userId
                           AND IsDeleted = 0;
                           """;
        using var connection = _context.CreateConnection();
        
        return await connection.QueryFirstOrDefaultAsync<User>(sql, new { UserId = userId });
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        const string sql = """
                           SELECT *
                           FROM USERS
                           AND IsDeleted = 0;
                            ORDER BY CreatedAt Desc;
                           """;
        using var connection = _context.CreateConnection();
        
        var users = await connection.QueryAsync<User>(sql);
        return users.ToList();
    }

    public async Task<User?> GetUserByPhoneNumberAsync(string phoneNumber)
    {
        const string sql = """
                           SELECT *
                           FROM USERS
                           WHERE PhoneNumber = @PhoneNumber
                           AND  IsDeleted = 0;
                           """;
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<User>(sql, new { PhoneNumber = phoneNumber });
    }

    public async Task AddAsync(User user)
    {
        const string sql = """
                           INSERT INTO Users
                           (
                               UserId,
                               FullName,
                               Email,
                               PhoneNumber,
                               PasswordHash,
                               Role,
                               IsEmailVerified,
                               IsPhoneNumberVerified,
                               IsActive,
                               IsDeleted,
                               CreatedAt
                           )
                           VALUES
                           (
                               @UserId,
                               @FullName,
                               @Email,
                               @PhoneNumber,
                               @PasswordHash,
                               @Role,
                               @IsEmailVerified,
                               @IsPhoneNumberVerified,
                               @IsActive,
                               @IsDeleted,
                               @CreatedAt
                           );
                           """;

        using var connection = _context.CreateConnection();

        await connection.ExecuteAsync(sql, user);
    }

    public async Task UpdateAsync(User user)
    {
        const string sql = """
                           UPDATE Users
                           SET
                               FullName = @FullName,
                               Email = @Email,
                               PhoneNumber = @PhoneNumber,
                               PasswordHash = @PasswordHash,
                               Role = @Role,
                               IsEmailVerified = @IsEmailVerified,
                               IsPhoneNumberVerified = @IsPhoneNumberVerified,
                               IsActive = @IsActive,
                               IsDeleted = @IsDeleted,
                               UpdatedAt = @UpdatedAt,
                               LastLoginAt = @LastLoginAt
                           WHERE Id = @Id;
                           """;

        using var connection = _context.CreateConnection();

        await connection.ExecuteAsync(sql, user);
    }
}