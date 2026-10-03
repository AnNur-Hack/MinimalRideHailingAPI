using System.Security.Claims;
using FluentValidation;
using MinimalRideHailingAPI.DTOs.Requests;
using MinimalRideHailingAPI.DTOs.Responses;
using MinimalRideHailingAPI.Services.Interfaces;

namespace MinimalRideHailingAPI.Endpoints;

public static class DriverEndpoints
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/api/driver")
            .RequireAuthorization()
            .WithTags("Driver");

        group.MapPost("/profile", async (
            ClaimsPrincipal user,
            CreateDriverProfileRequest request,
            IValidator<CreateDriverProfileRequest> validator,
            IDriverService driverService) =>
        {
            var userId = GetUserId(user);

            if (userId == null)
            {
                return Results.Unauthorized();
            }

            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = string.Join(
                    " ",
                    validationResult.Errors.Select(x => x.ErrorMessage));

                return Results.BadRequest(
                    ApiResponse.ResponseHelper.FailureResponse(errors));
            }

            var response = await driverService.CreateDriverProfileAsync(
                userId.Value,
                request);

            return Results.Ok(response);
        });

        group.MapGet("/profile", async (
            ClaimsPrincipal user,
            IDriverService driverService) =>
        {
            var userId = GetUserId(user);

            if (userId == null)
            {
                return Results.Unauthorized();
            }

            var response = await driverService.GetDriverProfileAsync(
                userId.Value);

            return Results.Ok(response);
        });

        group.MapPut("/profile", async (
            ClaimsPrincipal user,
            CreateDriverProfileRequest request,
            IValidator<CreateDriverProfileRequest> validator,
            IDriverService driverService) =>
        {
            var userId = GetUserId(user);

            if (userId == null)
            {
                return Results.Unauthorized();
            }

            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = string.Join(
                    " ",
                    validationResult.Errors.Select(x => x.ErrorMessage));

                return Results.BadRequest(
                    ApiResponse.ResponseHelper.FailureResponse(errors));
            }

            var response = await driverService.UpdateDriverProfileAsync(
                userId.Value,
                request);

            return Results.Ok(response);
        });

        group.MapPut("/availability", async (
            ClaimsPrincipal user,
            UpdateDriverAvailabilityRequest request,
            IValidator<UpdateDriverAvailabilityRequest> validator,
            IDriverService driverService) =>
        {
            var userId = GetUserId(user);

            if (userId == null)
            {
                return Results.Unauthorized();
            }

            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = string.Join(
                    " ",
                    validationResult.Errors.Select(x => x.ErrorMessage));

                return Results.BadRequest(
                    ApiResponse.ResponseHelper.FailureResponse(errors));
            }

            var response = await driverService.UpdateAvailabilityAsync(
                userId.Value,
                request);

            return Results.Ok(response);
        });

        group.MapPost("/vehicle", async (
            ClaimsPrincipal user,
            CreateVehicleRequest request,
            IValidator<CreateVehicleRequest> validator,
            IDriverService driverService) =>
        {
            var userId = GetUserId(user);

            if (userId == null)
            {
                return Results.Unauthorized();
            }

            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = string.Join(
                    " ",
                    validationResult.Errors.Select(x => x.ErrorMessage));

                return Results.BadRequest(
                    ApiResponse.ResponseHelper.FailureResponse(errors));
            }

            var response = await driverService.AddVehicleAsync(
                userId.Value,
                request);

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