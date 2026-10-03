using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MinimalRideHailingAPI.Domain.Entities;
using MinimalRideHailingAPI.DTOs.Responses;

namespace MinimalRideHailingAPI.Helpers;

public class JwtHelper
{
    private readonly IConfiguration _configuration;
    public JwtHelper(IConfiguration configuration)
    {
        _configuration = configuration;
    }


    public LoginResponse GenerateJwt(User user)
    {
        var expiryMinutes = int.Parse(_configuration["JWT:ExpirationInMinutes"]!);

        var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var claims = new List<Claim>()
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),

            new(ClaimTypes.Name, user.FullName),

            new(ClaimTypes.Email, user.Email),

            new(ClaimTypes.Role, user.Role.ToString()),

            new("phoneNumber", user.PhoneNumber),
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(

            issuer: _configuration["JWT:Issuer"]!,
            audience: _configuration["JWT:Audience"]!,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        return new LoginResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiresAt,
            User = new UserResponse
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role.ToString(),
                IsEmailVerified = user.IsEmailVerified
            }
        };

    }
}