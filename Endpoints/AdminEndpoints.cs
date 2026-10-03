using System.Security.Claims;
using MinimalRideHailingAPI.Services.Interfaces;

namespace MinimalRideHailingAPI.Endpoints;

public static class AdminEndpoints
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/api/admin")
            .RequireAuthorization(policy =>
                policy.RequireRole("Admin"))
            .WithTags("Admin");

        group.MapPut("/drivers/{driverUserId:int}/approve", async (
            ClaimsPrincipal user,
            int driverUserId,
            IAdminService adminService) =>
        {
            var adminUserId = GetUserId(user);

            if (adminUserId == null)
            {
                return Results.Unauthorized();
            }

            var response = await adminService.ApproveDriverAsync(
                adminUserId.Value,
                driverUserId);

            return Results.Ok(response);
        });

        group.MapPut("/users/{targetUserId:int}/status", async (
            ClaimsPrincipal user,
            int targetUserId,
            bool isActive,
            IAdminService adminService) =>
        {
            var adminUserId = GetUserId(user);

            if (adminUserId == null)
            {
                return Results.Unauthorized();
            }

            var response = await adminService.UpdateUserStatusAsync(
                adminUserId.Value,
                targetUserId,
                isActive);

            return Results.Ok(response);
        });

        group.MapGet("/users", async (
            ClaimsPrincipal user,
            IAdminService adminService) =>
        {
            var adminUserId = GetUserId(user);

            if (adminUserId == null)
            {
                return Results.Unauthorized();
            }

            var response = await adminService.GetUsersAsync(
                adminUserId.Value);

            return Results.Ok(response);
        });

        group.MapGet("/drivers", async (
            ClaimsPrincipal user,
            IAdminService adminService) =>
        {
            var adminUserId = GetUserId(user);

            if (adminUserId == null)
            {
                return Results.Unauthorized();
            }

            var response = await adminService.GetDriversAsync(
                adminUserId.Value);

            return Results.Ok(response);
        });
    }

    private static int? GetUserId(ClaimsPrincipal user)
    {
        var claim = user.FindFirst(ClaimTypes.NameIdentifier);

        if (claim == null)
        {
            return null;
        }

        return int.TryParse(claim.Value, out var userId)
            ? userId
            : null;
    }
}