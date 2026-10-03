using System.Security.Claims;
using FluentValidation;
using MinimalRideHailingAPI.DTOs.Requests;
using MinimalRideHailingAPI.DTOs.Responses;
using MinimalRideHailingAPI.Services.Interfaces;

namespace MinimalRideHailingAPI.Endpoints;

public static class PassengerEndpoints
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/api/passenger")
            .RequireAuthorization()
            .WithTags("Passenger");

        group.MapGet("/profile", async (
            ClaimsPrincipal user,
            IPassengerService passengerService) =>
        {
            var userId = GetUserId(user);

            if (userId == null)
            {
                return Results.Unauthorized();
            }

            var response = await passengerService.GetProfileAsync(userId.Value);

            return Results.Ok(response);
        });

        group.MapPut("/profile", async (
            ClaimsPrincipal user,
            UpdateProfileRequest request,
            IValidator<UpdateProfileRequest> validator,
            IPassengerService passengerService) =>
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

            var response = await passengerService.UpdateProfileAsync(
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