
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using BaseCleanWithJwt.Domain.Common.Settings;
using BaseCleanWithJwt.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BaseCleanWithJwt.Infrastructure.Identity;

public interface IJwtProvider
{
    string GenerateToken(UserModel UserModel);
    string GenerateRefreshToken();
}

public class JwtProvider(IOptions<JwtSettings> settings) : IJwtProvider
{
    private readonly JwtSettings _settings = settings.Value;
    public string GenerateToken(UserModel UserModel)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Email, UserModel.Email),
            new Claim(ClaimTypes.NameIdentifier, UserModel.Id.ToString()!),
        };

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_settings.IssuerSigningKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.ValidIssuer,
            audience: _settings.ValidAudience,
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        RandomNumberGenerator.Fill(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

}