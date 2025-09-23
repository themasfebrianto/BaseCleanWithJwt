namespace BaseCleanWithJwt.Domain.DTO.UserDTO;

public class UserResponseDTO
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string? FullName { get; set; }
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsEmailVerified { get; set; } = false;
    public int RoleId { get; set; } = 0; // 0: User, 1: Admin
    public DateTime? LastLoginAt { get; set; }
}