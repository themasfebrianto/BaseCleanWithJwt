using Microsoft.AspNetCore.Http;

namespace BaseCleanWithJwt.Domain.DTO.UserDTO;

public class UserRequestDTO
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public IFormFile? AvatarFile { get; set; }
    public string? FullName { get; set; }
    public string? Bio { get; set; }
    public int? RoleId { get; set; }
}