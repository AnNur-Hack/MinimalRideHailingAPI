using MinimalRideHailingAPI.Domain.Entities;
using MinimalRideHailingAPI.Domain.Enums;
using MinimalRideHailingAPI.DTOs.Requests;
using MinimalRideHailingAPI.DTOs.Responses;
using MinimalRideHailingAPI.Helpers;
using MinimalRideHailingAPI.Repositories.Interface;
using MinimalRideHailingAPI.Services.Interfaces;

namespace MinimalRideHailingAPI.Services.Implementations;

public class AuthService
    (ILogger<AuthService> logger, IUserRepository userRepository,
        JwtHelper jwtHelper, IOtpRepository otpRepository, IEmailService emailService, 
        IAuditLogRepository auditLogRepository, ISmsService smsService) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly JwtHelper _jwtHelper =  jwtHelper;
    private readonly IOtpRepository _otpRepository = otpRepository;
    private readonly IEmailService _emailService = emailService;
    private readonly IAuditLogRepository _auditLogRepository = auditLogRepository;
    private readonly ILogger<AuthService> _logger = logger;
    private readonly ISmsService _smsService = smsService;
    
    
public async Task<ApiResponse> RegisterAsync(RegisterRequest request)
{
    try
    {
        var existingUser =
            await _userRepository.GetUserByEmailAsync(request.Email);

        if (existingUser != null)
        {
            return ApiResponse.ResponseHelper.FailureResponse(
                "Email address already exists");
        }

        var existingPhoneNumber =
            await _userRepository.GetUserByPhoneNumberAsync(
                request.PhoneNumber);

        if (existingPhoneNumber != null)
        {
            return ApiResponse.ResponseHelper.FailureResponse(
                "Phone number already exists");
        }

        if (request.Role == UserRole.Admin)
        {
            return ApiResponse.ResponseHelper.FailureResponse(
                "Admin registration is not allowed");
        }

        var newUser = new User
        {
            UserId = UserIdGenerator.GenerateUserId(),
            FullName = $"{request.FirstName} {request.LastName}",
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            PasswordHash = PasswordHasher.EncryptPassword(request.Password),
            Role = request.Role,
            IsEmailVerified = false,
            IsPhoneNumberVerified = false,
            IsActive = true,
            IsDeleted = false
        };

        await _userRepository.AddAsync(newUser);
        

        await _otpRepository.InvalidatePreviousOtpsAsync(
            newUser.Id,
            "EmailVerification");

        var emailOtpCode = OtpGenerator.GenerateOtp();

        var emailOtp = new Otp
        {
            UserId = newUser.Id,
            OtpCode = emailOtpCode,
            IsUsed = false,
            Purpose = "EmailVerification",
            ExpiresAt = DateTime.UtcNow.AddMinutes(5)
        };

        await _otpRepository.AddAsync(emailOtp);

        await _emailService.SendOtpEmailAsync(
            newUser.Email,
            newUser.FullName,
            emailOtpCode);
        

        await _otpRepository.InvalidatePreviousOtpsAsync(
            newUser.Id,
            "PhoneVerification");

        var phoneOtpCode = OtpGenerator.GenerateOtp();

        var phoneOtp = new Otp
        {
            UserId = newUser.Id,
            OtpCode = phoneOtpCode,
            IsUsed = false,
            Purpose = "PhoneVerification",
            ExpiresAt = DateTime.UtcNow.AddMinutes(5)
        };

        await _otpRepository.AddAsync(phoneOtp);

        await _smsService.SendSmsAsync(
            newUser.PhoneNumber,
            $"Your RideHailingAPI verification code is {phoneOtpCode}. It expires in 5 minutes.");
        

        var auditLog = new AuditLog
        {
            UserId = newUser.Id,
            Action = "Registration",
            Status = "Success",
            Description = "User registered successfully"
        };

        await _auditLogRepository.AddAsync(auditLog);

        _logger.LogInformation(
            "User registered successfully: {UserId}",
            newUser.UserId);

        return ApiResponse.ResponseHelper.SuccessResponse(
            null,
            "Registration successful. OTPs have been sent to your email and phone");
    }
    catch (Exception ex)
    {
        _logger.LogError(
            ex,
            "An error occurred during registration for {Email}",
            request.Email);

        return ApiResponse.ResponseHelper.FailureResponse(
            "An error occurred during registration");
    }
}

    public async Task<ApiResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);

            if (user == null)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    "Invalid email or password");
            }

            if (user.IsDeleted || !user.IsActive)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    "Your account is inactive");
            }

            if (!user.IsEmailVerified)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    "Please verify your email before logging in");
            }

            var passwordIsValid = PasswordHasher.VerifyHashedPassword(
                request.Password,
                user.PasswordHash);

            if (!passwordIsValid)
            {
                await _auditLogRepository.AddAsync(new AuditLog
                {
                    UserId = user.Id,
                    Action = "Login",
                    Status = "Failed",
                    Description = "Failed login attempt"
                });

                return ApiResponse.ResponseHelper.FailureResponse(
                    "Invalid email or password");
            }

            var loginResponse = _jwtHelper.GenerateJwt(user);

            await _auditLogRepository.AddAsync(new AuditLog
            {
                UserId = user.Id,
                Action = "Login",
                Status = "Success",
                Description = "User logged in successfully"
            });

            return ApiResponse.ResponseHelper.SuccessResponse(
                loginResponse,
                "Login successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");

            return ApiResponse.ResponseHelper.FailureResponse(
                "An error occurred during login");
        }
        
    }

    public async Task<ApiResponse> VerifyOtpAsync(
        VerifyOtpRequest request)
    {
        try
        {
            var user = await _userRepository
                .GetUserByEmailAsync(request.Email);

            if (user == null)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    "User not found");
            }

            var otp = await _otpRepository.GetValidOtpAsync(
                user.Id,
                request.OtpCode,
                "EmailVerification");

            if (otp == null)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    "Invalid or expired OTP");
            }

            user.IsEmailVerified = true;

            otp.IsUsed = true;
            otp.VerifiedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            await _otpRepository.UpdateAsync(otp);
            
            await _emailService.SendWelcomeEmailAsync(
                user.Email,
                user.FullName);

            await _auditLogRepository.AddAsync(new AuditLog
            {
                UserId = user.Id,
                Action = "EmailVerification",
                Status = "Success",
                Description = "User email verified successfully"
            });

            return ApiResponse.ResponseHelper.SuccessResponse(
                null,
                "Email verified successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error verifying OTP for {Email}",
                request.Email);

            return ApiResponse.ResponseHelper.FailureResponse(
                "An error occurred while verifying OTP");
        }
    }

    public async Task<ApiResponse> ResendOtpAsync(ResendOtpRequest request)
    {
        try
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);

            if (user == null)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    "Unable to process request");
            }

            if (user.IsEmailVerified)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    "Email is already verified");
            }

            await _otpRepository.InvalidatePreviousOtpsAsync(
                user.Id,
                "EmailVerification");

            var otpCode = OtpGenerator.GenerateOtp();

            var otp = new Otp
            {
                UserId = user.Id,
                OtpCode = otpCode,
                IsUsed = false,
                Purpose = "EmailVerification",
                ExpiresAt = DateTime.UtcNow.AddMinutes(5)
            };

            await _otpRepository.AddAsync(otp);

            await _emailService.SendOtpEmailAsync(
                user.Email,
                user.FullName,
                otpCode);

            return ApiResponse.ResponseHelper.SuccessResponse(
                null,
                "A new OTP has been sent to your email");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resending OTP");

            return ApiResponse.ResponseHelper.FailureResponse(
                "An error occurred while resending OTP");
        }
        
    }

    public async Task<ApiResponse> ResetPasswordAsync(ResetPasswordRequest request)
    {
        try
        {
            var user = await _userRepository
                .GetUserByEmailAsync(request.Email);

            if (user == null)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    "Invalid email or OTP");
            }

            var otp = await _otpRepository.GetValidOtpAsync(
                user.Id,
                request.OtpCode,
                "PasswordReset");

            if (otp == null)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    "Invalid or expired OTP");
            }

            user.PasswordHash = PasswordHasher.EncryptPassword(
                request.NewPassword);

            user.UpdatedAt = DateTime.UtcNow;

            otp.IsUsed = true;
            otp.VerifiedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            await _otpRepository.UpdateAsync(otp);

            await _auditLogRepository.AddAsync(new AuditLog
            {
                UserId = user.Id,
                Action = "PasswordReset",
                Status = "Success",
                Description = "User password was reset successfully"
            });

            _logger.LogInformation(
                "Password reset successfully for user {UserId}",
                user.UserId);

            return ApiResponse.ResponseHelper.SuccessResponse(
                null,
                "Password reset successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An error occurred while resetting password for {Email}",
                request.Email);

            return ApiResponse.ResponseHelper.FailureResponse(
                "An error occurred while resetting password");
        }
        
    }

    public async Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        try
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);

            if (user != null)
            {
                await _otpRepository.InvalidatePreviousOtpsAsync(
                    user.Id,
                    "PasswordReset");

                var otpCode = OtpGenerator.GenerateOtp();

                var otp = new Otp
                {
                    UserId = user.Id,
                    OtpCode = otpCode,
                    IsUsed = false,
                    Purpose = "PasswordReset",
                    ExpiresAt = DateTime.UtcNow.AddMinutes(5)
                };

                await _otpRepository.AddAsync(otp);

                await _emailService.SendPasswordResetEmailAsync(
                    user.Email,
                    user.FullName,
                    otpCode);
            }

            return ApiResponse.ResponseHelper.SuccessResponse(
                null,
                "If an account exists with this email, a password reset OTP has been sent.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing forgot password request");

            return ApiResponse.ResponseHelper.FailureResponse(
                "An error occurred while processing your request");
        }
        
    }

    public async Task<ApiResponse> ChangePasswordAsync(ChangePasswordRequest request)
    {
        try
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);

            if (user == null)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    "User not found");
            }

            var passwordIsValid = PasswordHasher.VerifyHashedPassword(
                request.OldPassword,
                user.PasswordHash);

            if (!passwordIsValid)
            {
                return ApiResponse.ResponseHelper.FailureResponse(
                    "Current password is incorrect");
            }

            user.PasswordHash = PasswordHasher.EncryptPassword(
                request.NewPassword);

            await _userRepository.UpdateAsync(user);

            await _auditLogRepository.AddAsync(new AuditLog
            {
                UserId = user.Id,
                Action = "PasswordChange",
                Status = "Success",
                Description = "User changed password successfully"
            });

            return ApiResponse.ResponseHelper.SuccessResponse(
                null,
                "Password changed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password");

            return ApiResponse.ResponseHelper.FailureResponse(
                "An error occurred while changing password");
        }
        
    }
}