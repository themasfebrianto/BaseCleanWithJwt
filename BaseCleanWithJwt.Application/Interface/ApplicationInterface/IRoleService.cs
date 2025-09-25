
using BaseCleanWithJwt.Domain.DTO.RoleDTO;
using BaseCleanWithJwt.Domain.Entities;

namespace BaseCleanWithJwt.Application.Interface.ApplicationInterface;

public interface IRoleService
{
    Task<RoleModel> GetRoleById(Guid id);
    Task<RoleModel> GetRoleByName(string name);
    Task<List<RoleModel>> GetAllRoles();
    Task<RoleModel> CreateRole(RoleRequestDTO request);
    Task<RoleModel> UpdateRole(RoleRequestDTO request);
    Task<bool> DeleteRole(Guid id);
}