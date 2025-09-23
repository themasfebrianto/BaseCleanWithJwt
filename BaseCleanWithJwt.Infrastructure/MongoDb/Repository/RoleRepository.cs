using BaseCleanWithJwt.Application.Interface.RepositoryInterface;
using BaseCleanWithJwt.Domain.Entities;

namespace BaseCleanWithJwt.Infrastructure.MongoDb.Repository;

public class RoleRepository : IRoleRepository
{
    public Task<RoleModel> CreateRole(RoleModel role)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteRole(int id)
    {
        throw new NotImplementedException();
    }

    public Task<List<RoleModel>> GetAllRoles()
    {
        throw new NotImplementedException();
    }

    public Task<RoleModel> GetRoleById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<RoleModel> GetRoleByName(string name)
    {
        throw new NotImplementedException();
    }

    public Task<RoleModel> UpdateRole(RoleModel role)
    {
        throw new NotImplementedException();
    }
}