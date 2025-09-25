using BaseCleanWithJwt.Domain.DTO.RoleDTO;
using BaseCleanWithJwt.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace BaseCleanWithJwt.Application.Mapping;
#pragma warning disable RMG012,RMG020, RMG066
[Mapper]
public static partial class RoleMopper
{
    public static partial RoleModel MapToModel(this RoleRequestDTO role);
    public static RoleModel MapToRoleModel(this RoleRequestDTO request)
    {
        var mapped = request.MapToModel();
        mapped.CreatedAt = DateTime.UtcNow;
        mapped.UpdatedAt = DateTime.UtcNow;
        return mapped;
    }
}