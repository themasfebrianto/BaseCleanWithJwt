using BaseCleanWithJwt.Application.Interface.RepositoryInterface;
using BaseCleanWithJwt.Domain.DTO.UserDTO;
using BaseCleanWithJwt.Domain.Entities;
using BaseCleanWithJwt.Infrastructure.MongoDb.Common;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BaseCleanWithJwt.Infrastructure.MongoDb.Repository;

public class UserRepository(IMongoDbContext context, IOptions<MongoDbSettings> settings) : IUserRepository
{
    private readonly IMongoCollection<UserModel> _collection = context.GetMongoCollection<UserModel>(settings.Value.UserCollection);
    public async Task<UserModel?> GetByEmailAsync(string email) =>
        await _collection.Find(x => x.Email == email).FirstOrDefaultAsync();

    public async Task<UserModel?> GetByIdAsync(Guid id) =>
        await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<UserModel> InsertOneAsync(UserModel request)
    {
        await _collection.InsertOneAsync(request);
        return request;
    }

    public async Task<UserModel> UpdateOneAsync(UserModel user)
    {
        if (user.Id == Guid.Empty)
            throw new ArgumentException("User.Id is required for update");

        var filter = Builders<UserModel>.Filter.Eq(u => u.Id, user.Id);

        var update = UpdateDefinitionBuilder.Create(user,
            nameof(UserModel.Id),
            nameof(UserModel.CreatedAt));

        update = Builders<UserModel>.Update.Combine(
            update,
            Builders<UserModel>.Update.Set(u => u.UpdatedAt, DateTime.UtcNow)
        );

        await _collection.UpdateOneAsync(filter, update);
        return user;
    }

    public async Task DeleteByIdAsync(Guid id) => await _collection.DeleteOneAsync(x => x.Id == id);

    public Task<List<UserModel>> GetAllAsync()
    {
        return _collection.Find(_ => true).ToListAsync();
    }

    public async Task DeleteOneAsync(Guid id)
    {
        await _collection.DeleteOneAsync(x => x.Id == id);
    }
}