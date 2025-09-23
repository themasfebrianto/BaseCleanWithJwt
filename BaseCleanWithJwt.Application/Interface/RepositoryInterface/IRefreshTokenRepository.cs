using BaseCleanWithJwt.Domain.Entities;

namespace BaseCleanWithJwt.Application.Interface.RepositoryInterface;

public interface IRefreshTokensRepository
{
    Task<RefreshTokenModel?> GetByTokenAsync(string token);
    Task UpdateRefreshTokenAsync(string id, string token, DateTime expiresAt);
    Task DeleteByIdAsync(string id);
}