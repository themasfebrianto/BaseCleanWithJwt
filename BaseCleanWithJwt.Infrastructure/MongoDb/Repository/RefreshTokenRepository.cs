using BaseCleanWithJwt.Application.Interface.RepositoryInterface;
using BaseCleanWithJwt.Domain.Entities;

namespace BaseCleanWithJwt.Infrastructure.MongoDb.Repository;

public class RefreshTokenRepository : IRefreshTokensRepository
{
    public Task DeleteByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<RefreshTokenModel?> GetByTokenAsync(string token)
    {
        throw new NotImplementedException();
    }

    public Task UpdateRefreshTokenAsync(string id, string token, DateTime expiresAt)
    {
        throw new NotImplementedException();
    }
}