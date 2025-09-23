using BaseCleanWithJwt.Domain.Entities;

namespace BaseCleanWithJwt.Application.Interface.RepositoryInterface;

public interface IRoleRepository
{
    Task<RoleModel> GetRoleById(int id);
    Task<RoleModel> GetRoleByName(string name);
    Task<List<RoleModel>> GetAllRoles();
    Task<RoleModel> CreateRole(RoleModel role);
    Task<RoleModel> UpdateRole(RoleModel role);
    Task<bool> DeleteRole(int id);
}