
using BaseCleanWithJwt.Domain.DTO.AuthDTO;
using BaseCleanWithJwt.Domain.DTO.UserDTO;

namespace BaseCleanWithJwt.Application.Interface.ApplicationInterface;

public interface IAuthService
{
    Task<LoginResponseDTO?> RegisterAsync(UserRequestDTO request);
    Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO request);
    Task<LoginResponseDTO?> RefreshTokenAsync(string refreshToken);
}