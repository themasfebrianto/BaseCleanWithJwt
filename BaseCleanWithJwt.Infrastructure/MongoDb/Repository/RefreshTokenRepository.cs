using BaseCleanWithJwt.Application.Interface.InfrastructureInterface;
using BaseCleanWithJwt.Domain.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BaseCleanWithJwt.Infrastructure.MongoDb.Repository;

public class RefreshTokenRepository(IMongoDbContext context, IOptions<MongoDbSettings> settings) : IRefreshTokenRepository
{
    private readonly IMongoCollection<RefreshTokenModel> _collection =
        context.GetMongoCollection<RefreshTokenModel>(settings.Value.RefreshTokensCollection);

    public async Task UpdateRefreshTokenAsync(Guid userId, string token, DateTime expiresAt)
    {
        var filter = Builders<RefreshTokenModel>.Filter.Eq(u => u.UserId, userId);
        var update = Builders<RefreshTokenModel>.Update
            .Set(u => u.Token, token)
            .Set(u => u.ExpiresAt, expiresAt)
            .Set(u => u.CreatedAt, DateTime.UtcNow);

        await _collection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
    }

    public async Task<RefreshTokenModel?> GetByTokenAsync(string token) =>
        await _collection.Find(x => x.Token == token).FirstOrDefaultAsync();

    public async Task DeleteByIdAsync(Guid id)
    {
        var filter = Builders<RefreshTokenModel>.Filter.Eq(x => x.Id, id);
        await _collection.DeleteOneAsync(filter);
    }
}
