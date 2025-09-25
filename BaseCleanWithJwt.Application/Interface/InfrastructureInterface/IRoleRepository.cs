using BaseCleanWithJwt.Domain.Entities;

namespace BaseCleanWithJwt.Application.Interface.InfrastructureInterface;

public interface IRoleRepository
{
    Task<RoleModel> GetRoleById(Guid id);
    Task<RoleModel> GetRoleByName(string name);
    Task<List<RoleModel>> GetAllRoles();
    Task<RoleModel> CreateRole(RoleModel role);
    Task<RoleModel> UpdateRole(RoleModel role);
    Task<bool> DeleteRole(Guid id);
}