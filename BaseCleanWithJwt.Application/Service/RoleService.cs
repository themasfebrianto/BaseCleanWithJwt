using BaseCleanWithJwt.Application.Interface.ApplicationInterface;
using BaseCleanWithJwt.Application.Interface.InfrastructureInterface;
using BaseCleanWithJwt.Application.Mapping;
using BaseCleanWithJwt.Domain.DTO.RoleDTO;
using BaseCleanWithJwt.Domain.Entities;

namespace BaseCleanWithJwt.Application.Service;

public class RoleService(IRoleRepository roleRepository) : IRoleService
{
    public async Task<RoleModel> CreateRole(RoleRequestDTO request)
    {
        var newRole = request.MapToRoleModel();
        return await roleRepository.CreateRole(newRole);
    }

    public Task<bool> DeleteRole(Guid id)
    {
        var role = roleRepository.GetRoleById(id) ?? throw new Exception("Role not found");
        return roleRepository.DeleteRole(id);
    }

    public Task<List<RoleModel>> GetAllRoles()
    {
        return roleRepository.GetAllRoles();
    }

    public Task<RoleModel> GetRoleById(Guid id)
    {
        return roleRepository.GetRoleById(id);
    }

    public Task<RoleModel> GetRoleByName(string name)
    {
        return roleRepository.GetRoleByName(name);
    }

    public Task<RoleModel> UpdateRole(RoleRequestDTO request)
    {
        return roleRepository.UpdateRole(request.MapToRoleModel());
    }
}