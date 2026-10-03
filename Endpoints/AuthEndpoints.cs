using FluentValidation;
using MinimalRideHailingAPI.DTOs.Requests;
using MinimalRideHailingAPI.DTOs.Responses;
using MinimalRideHailingAPI.Services.Interfaces;

namespace MinimalRideHailingAPI.Endpoints;

public static class AuthEndpoints
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/api/auth")
            .WithTags("Authentication");

        group.MapPost("/register", async (
            RegisterRequest request,
            IValidator<RegisterRequest> validator,
            IAuthService authService) =>
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = string.Join(
                    " ",
                    validationResult.Errors.Select(x => x.ErrorMessage));

                return Results.BadRequest(
                    ApiResponse.ResponseHelper.FailureResponse(errors));
            }

            var response = await authService.RegisterAsync(request);

            return Results.Ok(response);
        });

        group.MapPost("/login", async (
            LoginRequest request,
            IValidator<LoginRequest> validator,
            IAuthService authService) =>
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = string.Join(
                    " ",
                    validationResult.Errors.Select(x => x.ErrorMessage));

                return Results.BadRequest(
                    ApiResponse.ResponseHelper.FailureResponse(errors));
            }

            var response = await authService.LoginAsync(request);

            return Results.Ok(response);
        });

        group.MapPost("/verify-otp", async (
            VerifyOtpRequest request,
            IValidator<VerifyOtpRequest> validator,
            IAuthService authService) =>
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = string.Join(
                    " ",
                    validationResult.Errors.Select(x => x.ErrorMessage));

                return Results.BadRequest(
                    ApiResponse.ResponseHelper.FailureResponse(errors));
            }

            var response = await authService.VerifyOtpAsync(request);

            return Results.Ok(response);
        });

        group.MapPost("/resend-otp", async (
            ResendOtpRequest request,
            IValidator<ResendOtpRequest> validator,
            IAuthService authService) =>
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = string.Join(
                    " ",
                    validationResult.Errors.Select(x => x.ErrorMessage));

                return Results.BadRequest(
                    ApiResponse.ResponseHelper.FailureResponse(errors));
            }

            var response = await authService.ResendOtpAsync(request);

            return Results.Ok(response);
        });

        group.MapPost("/forgot-password", async (
            ForgotPasswordRequest request,
            IValidator<ForgotPasswordRequest> validator,
            IAuthService authService) =>
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = string.Join(
                    " ",
                    validationResult.Errors.Select(x => x.ErrorMessage));

                return Results.BadRequest(
                    ApiResponse.ResponseHelper.FailureResponse(errors));
            }

            var response = await authService.ForgotPasswordAsync(request);

            return Results.Ok(response);
        });

        group.MapPost("/reset-password", async (
            ResetPasswordRequest request,
            IValidator<ResetPasswordRequest> validator,
            IAuthService authService) =>
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = string.Join(
                    " ",
                    validationResult.Errors.Select(x => x.ErrorMessage));

                return Results.BadRequest(
                    ApiResponse.ResponseHelper.FailureResponse(errors));
            }

            var response = await authService.ResetPasswordAsync(request);

            return Results.Ok(response);
        });

        group.MapPost("/change-password", async (
            ChangePasswordRequest request,
            IValidator<ChangePasswordRequest> validator,
            IAuthService authService) =>
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var errors = string.Join(
                    " ",
                    validationResult.Errors.Select(x => x.ErrorMessage));

                return Results.BadRequest(
                    ApiResponse.ResponseHelper.FailureResponse(errors));
            }

            var response = await authService.ChangePasswordAsync(request);

            return Results.Ok(response);
        });
    }
}