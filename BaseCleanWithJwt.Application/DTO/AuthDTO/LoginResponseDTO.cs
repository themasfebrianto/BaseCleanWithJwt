namespace BaseCleanWithJwt.Domain.DTO.AuthDTO;

public class LoginResponseDTO
{
    public string RefreshToken { get; set; } = null!;
    public string AccessToken { get; set; } = null!;
    public DateTime RefreshTokenExpires { get; set; }
    public DateTime AccessTokenExpires { get; set; }
}