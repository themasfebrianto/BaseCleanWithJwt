using BaseCleanWithJwt.Api.Common.Response;
using BaseCleanWithJwt.Application.Interface.ApplicationInterface;
using BaseCleanWithJwt.Domain.DTO.AuthDTO;
using BaseCleanWithJwt.Domain.DTO.UserDTO;
using Microsoft.AspNetCore.Mvc;

namespace BaseCleanWithJwt.Api.Controller;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
    {
        try
        {
            var result = await authService.LoginAsync(request);
            if (result == null)
                return CustomResponse.StatusCode(StatusCodes.Status401Unauthorized, "Invalid email or password.");

            return CustomResponse.StatusCode(StatusCodes.Status200OK, "Login successful.", result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Login failed: {Message}", ex.Message);
            return CustomResponse.Exception(ex);
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRequestDTO request)
    {
        try
        {
            var result = await authService.RegisterAsync(request);
            if (result == null)
                return CustomResponse.StatusCode(StatusCodes.Status400BadRequest, "Registration failed. Email may already be in use.");

            return CustomResponse.StatusCode(StatusCodes.Status201Created, "Registration successful.", result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Registration failed: {Message}", ex.Message);
            return CustomResponse.Exception(ex);
        }
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
    {
        try
        {
            var result = await authService.RefreshTokenAsync(refreshToken);
            if (result == null)
                return CustomResponse.StatusCode(StatusCodes.Status401Unauthorized, "Invalid refresh token.");

            return CustomResponse.StatusCode(StatusCodes.Status200OK, "Token refreshed successfully.", result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Token refresh failed: {Message}", ex.Message);
            return CustomResponse.Exception(ex);
        }
    }
}