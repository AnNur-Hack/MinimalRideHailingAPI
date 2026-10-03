using MinimalRideHailingAPI.Domain.Entities;

namespace MinimalRideHailingAPI.Repositories.Interface;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog auditLog);

}