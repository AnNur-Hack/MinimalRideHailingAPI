using FluentValidation;
using MinimalRideHailingAPI.Data;
using MinimalRideHailingAPI.Repositories.Implementation;
using MinimalRideHailingAPI.Repositories.Interface;
using MinimalRideHailingAPI.Services.Implementations;
using MinimalRideHailingAPI.Services.Interfaces;

namespace MinimalRideHailingAPI.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<DapperContext>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRideRepository, RideRepository>();
        services.AddScoped<IDriverProfileRepository, DriverProfileRepository>();
        services.AddScoped<IOtpRepository, OtpRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IVehicleRepository, VehicleRepository>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IDriverService, DriverService>();
        services.AddScoped<IPassengerService, PassengerService>();
        services.AddScoped<IRideService, RideService>();
        services.AddScoped<IAdminService, AdminService>();

        services.AddValidatorsFromAssemblyContaining<
            RegisterRequestValidator>();

        return services;
    }
}