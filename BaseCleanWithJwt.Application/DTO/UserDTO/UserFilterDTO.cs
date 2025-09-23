using BaseCleanWithJwt.Domain.Common;

namespace BaseCleanWithJwt.Domain.DTO.UserDTO;

public class UserFilterDTO : FilterBase
{
    public string? Id { get; set; }
    public string? Email { get; set; }
    public string? FullName { get; set; }
}