
using BaseCleanWithJwt.Domain.DTO.UserDTO;
using BaseCleanWithJwt.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace BaseCleanWithJwt.Application.Mapping;
#pragma warning disable RMG012,RMG020, RMG066
[Mapper]
public static partial class UserMapper
{
    public static partial UserModel MapToUserModel(this UserRequestDTO user);
    public static partial UserResponseDTO MapToUserResponseDTO(this UserModel user);
}