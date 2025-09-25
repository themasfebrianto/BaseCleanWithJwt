using BaseCleanWithJwt.Application.Interface.ApplicationInterface;
using BaseCleanWithJwt.Application.Interface.InfrastructureInterface;
using BaseCleanWithJwt.Domain.Common.Settings;
using BaseCleanWithJwt.Domain.DTO.AuthDTO;
using BaseCleanWithJwt.Domain.DTO.UserDTO;
using Microsoft.Extensions.Options;

namespace BaseCleanWithJwt.Application.Service;

public class AuthService(
    IRefreshTokenRepository refreshTokensRepository,
    IOptions<JwtSettings> jwtSettings,
    IUserRepository userRepository,
    IJwtProvider jwtProvider,
    IUserService userService
    ) : IAuthService
{
    public async Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO request)
    {
        var user = await userRepository.GetByEmailAsync(request.Email);
        if (user == null) return null;

        if (!VerifyPassword(request.Password, user.PasswordHash))
            return null;

        var accessToken = jwtProvider.GenerateToken(user);
        var refreshToken = jwtProvider.GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.Add(jwtSettings.Value.RefreshTokenExpiration);
        await refreshTokensRepository.UpdateRefreshTokenAsync((Guid)user.Id!, refreshToken, refreshTokenExpiry);

        return new LoginResponseDTO
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpires = DateTime.UtcNow.Add(jwtSettings.Value.Expiration),
            RefreshTokenExpires = refreshTokenExpiry,
        };
    }

    public async Task<LoginResponseDTO?> RegisterAsync(UserRequestDTO request)
    {
        var existingUser = await userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
            throw new InvalidOperationException("Email already registered.");

        var user = await userService.InsertOneAsync(request);
        var userModel = await userRepository.GetByIdAsync(user.Id) ?? throw new InvalidOperationException("User not found after registration.");

        var accessToken = jwtProvider.GenerateToken(userModel);
        var refreshToken = jwtProvider.GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.Add(jwtSettings.Value.RefreshTokenExpiration);
        await refreshTokensRepository.UpdateRefreshTokenAsync(user.Id, refreshToken, refreshTokenExpiry);

        return new LoginResponseDTO
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpires = DateTime.UtcNow.Add(jwtSettings.Value.Expiration),
            RefreshTokenExpires = refreshTokenExpiry,
        };
    }

    public async Task<LoginResponseDTO?> RefreshTokenAsync(string refreshToken)
    {
        var refreshTokenData = await refreshTokensRepository.GetByTokenAsync(refreshToken);
        if (refreshTokenData == null || refreshTokenData.Token == null || refreshTokenData.ExpiresAt < DateTime.UtcNow)
            return null;

        var user = await userRepository.GetByIdAsync(refreshTokenData.UserId) ?? throw new InvalidOperationException("User not found.");
        var accessToken = jwtProvider.GenerateToken(user);

        var newRefreshToken = jwtProvider.GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.Add(jwtSettings.Value.RefreshTokenExpiration);

        await refreshTokensRepository.UpdateRefreshTokenAsync((Guid)user.Id!, newRefreshToken, refreshTokenExpiry);

        return new LoginResponseDTO
        {
            AccessToken = accessToken,
            AccessTokenExpires = DateTime.UtcNow.Add(jwtSettings.Value.Expiration),
            RefreshToken = newRefreshToken,
            RefreshTokenExpires = refreshTokenExpiry
        };
    }

    private static bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
