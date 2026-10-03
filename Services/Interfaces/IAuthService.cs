using MinimalRideHailingAPI.DTOs.Requests;
using MinimalRideHailingAPI.DTOs.Responses;

namespace MinimalRideHailingAPI.Services.Interfaces;

public interface IAuthService
{
    Task<ApiResponse> RegisterAsync(RegisterRequest request);
    Task<ApiResponse> LoginAsync(LoginRequest request);
    Task<ApiResponse> VerifyOtpAsync(VerifyOtpRequest request);
    Task<ApiResponse> ResendOtpAsync(ResendOtpRequest request);
    Task<ApiResponse> ResetPasswordAsync(ResetPasswordRequest request);
    Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordRequest request);
    Task<ApiResponse> ChangePasswordAsync(ChangePasswordRequest request);
}