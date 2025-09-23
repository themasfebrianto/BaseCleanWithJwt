
using BaseCleanWithJwt.Domain.DTO.RoleDTO;
using BaseCleanWithJwt.Domain.Entities;

namespace BaseCleanWithJwt.Application.Interface.ServiceInterface;

public interface IRoleService
{
    Task<RoleModel> GetRoleById(int id);
    Task<RoleModel> GetRoleByName(string name);
    Task<List<RoleModel>> GetAllRoles();
    Task<RoleModel> CreateRole(RoleRequestDTO request);
    Task<RoleModel> UpdateRole(RoleRequestDTO request);
    Task<bool> DeleteRole(int id);
}