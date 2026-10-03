using System.Security.Cryptography;

namespace MinimalRideHailingAPI.Helpers;

public static class UserIdGenerator
{
    public static string GenerateUserId(string prefix = "USR")
    {
        var  randomNumber = RandomNumberGenerator.GetInt32(100000, 1000000);
        return $"{prefix}{randomNumber}";
    }
}