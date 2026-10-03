using Dapper;
using MinimalRideHailingAPI.Data;
using MinimalRideHailingAPI.Domain.Entities;
using MinimalRideHailingAPI.Repositories.Interface;

namespace MinimalRideHailingAPI.Repositories.Implementation;

public class AuditLogRepository(DapperContext context) : IAuditLogRepository
{
    private readonly DapperContext _context = context;
    public async Task AddAsync(AuditLog auditLog)
    {
        const string sql = """
                           INSERT INTO AuditLogs
                           (
                               UserId,
                               Action,
                               Status,
                               Description,
                               CreatedAt
                           )
                           VALUES
                           (
                               @UserId,
                               @Action,
                               @Status,
                               @Description,
                               @CreatedAt
                           );
                           """;

        using var connection = _context.CreateConnection();

        await connection.ExecuteAsync(sql, auditLog);
    }
    
}
