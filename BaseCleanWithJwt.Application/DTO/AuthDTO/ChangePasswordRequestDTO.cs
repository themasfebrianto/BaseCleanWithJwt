namespace BaseCleanWithJwt.Domain.DTO.AuthDTO;

public class ChangePasswordRequestDTO
{
    public string Email { get; set; } = null!;
    public string OldPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}