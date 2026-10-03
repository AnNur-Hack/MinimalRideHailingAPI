using System.Security.Claims;
using FluentValidation;
using MinimalRideHailingAPI.DTOs.Requests;
using MinimalRideHailingAPI.DTOs.Responses;
using MinimalRideHailingAPI.Services.Interfaces;

namespace MinimalRideHailingAPI.Endpoints;

public static class RideEndpoints
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/api/rides")
            .RequireAuthorization()
            .WithTags("Rides");

        group.MapPost("/", async (
            ClaimsPrincipal user,
            CreateRideRequest request,
            IValidator<CreateRideRequest> validator,
            IRideService rideService) =>
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

            var response = await rideService.CreateRideAsync(
                userId.Value,
                request);

            return Results.Ok(response);
        });

        group.MapGet("/{rideReference}", async (
            ClaimsPrincipal user,
            string rideReference,
            IRideService rideService) =>
        {
            var userId = GetUserId(user);

            if (userId == null)
            {
                return Results.Unauthorized();
            }

            var response = await rideService.GetRideByReferenceAsync(
                userId.Value,
                rideReference);

            return Results.Ok(response);
        });

        group.MapGet("/passenger", async (
            ClaimsPrincipal user,
            IRideService rideService) =>
        {
            var userId = GetUserId(user);

            if (userId == null)
            {
                return Results.Unauthorized();
            }

            var response = await rideService.GetPassengerRidesAsync(
                userId.Value);

            return Results.Ok(response);
        });

        group.MapGet("/driver", async (
            ClaimsPrincipal user,
            IRideService rideService) =>
        {
            var userId = GetUserId(user);

            if (userId == null)
            {
                return Results.Unauthorized();
            }

            var response = await rideService.GetDriverRidesAsync(
                userId.Value);

            return Results.Ok(response);
        });

        group.MapGet("/available", async (
            ClaimsPrincipal user,
            IRideService rideService) =>
        {
            var userId = GetUserId(user);

            if (userId == null)
            {
                return Results.Unauthorized();
            }

            var response = await rideService.GetAvailableRidesAsync(
                userId.Value);

            return Results.Ok(response);
        });

        group.MapPost("/{rideReference}/accept", async (
            ClaimsPrincipal user,
            string rideReference,
            IRideService rideService) =>
        {
            var userId = GetUserId(user);

            if (userId == null)
            {
                return Results.Unauthorized();
            }

            var response = await rideService.AcceptRideAsync(
                userId.Value,
                rideReference);

            return Results.Ok(response);
        });

        group.MapPut("/{rideReference}/status", async (
            ClaimsPrincipal user,
            string rideReference,
            UpdateRideStatusRequest request,
            IValidator<UpdateRideStatusRequest> validator,
            IRideService rideService) =>
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

            var response = await rideService.UpdateRideStatusAsync(
                userId.Value,
                rideReference,
                request);

            return Results.Ok(response);
        });

        group.MapPost("/{rideReference}/cancel", async (
            ClaimsPrincipal user,
            string rideReference,
            IRideService rideService) =>
        {
            var userId = GetUserId(user);

            if (userId == null)
            {
                return Results.Unauthorized();
            }

            var response = await rideService.CancelRideAsync(
                userId.Value,
                rideReference);

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