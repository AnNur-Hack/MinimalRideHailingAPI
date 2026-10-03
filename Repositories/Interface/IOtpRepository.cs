using MinimalRideHailingAPI.Domain.Entities;

namespace MinimalRideHailingAPI.Repositories.Interface;

public interface IOtpRepository
{
    Task AddAsync(Otp otp);
    Task<Otp?> GetLatestOtpAsync(int userId, string purpose);
    Task<Otp?> GetValidOtpAsync(int userId, string otp, string purpose); 
    Task InvalidatePreviousOtpsAsync(int userId, string purpose);
    Task UpdateAsync(Otp otp);
}