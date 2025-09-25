using BaseCleanWithJwt.Application.Interface.InfrastructureInterface;
using BaseCleanWithJwt.Domain.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BaseCleanWithJwt.Infrastructure.MongoDb.Repository;

public class RoleRepository(IMongoDbContext context, IOptions<MongoDbSettings> settings) : IRoleRepository
{
    private readonly IMongoCollection<RoleModel> _collection = context.GetMongoCollection<RoleModel>(settings.Value.RoleCollection);
    public async Task<RoleModel> CreateRole(RoleModel role)
    {
        await _collection.InsertOneAsync(role);
        return role;
    }

    public async Task<bool> DeleteRole(Guid id)
    {
        await _collection.DeleteOneAsync(u => u.Id == id);
        return true;
    }

    public async Task<List<RoleModel>> GetAllRoles()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public Task<RoleModel> GetRoleById(Guid id)
    {
        return _collection.Find(u => u.Id == id).FirstOrDefaultAsync();
    }

    public Task<RoleModel> GetRoleByName(string name)
    {
        return _collection.Find(u => u.Name == name).FirstOrDefaultAsync();
    }

    public Task<RoleModel> UpdateRole(RoleModel role)
    {
        return _collection.FindOneAndReplaceAsync(u => u.Id == role.Id, role);
    }
}