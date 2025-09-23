
using BaseCleanWithJwt.Domain.DTO.UserDTO;
using BaseCleanWithJwt.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace BaseCleanWithJwt.Application.Mapping;

[Mapper]
public static partial class UserMapper
{
    public static partial UserModel MapToUserModel(this UserRequestDTO user);
    public static partial UserResponseDTO MapToUserResponseDTO(this UserModel user);
}