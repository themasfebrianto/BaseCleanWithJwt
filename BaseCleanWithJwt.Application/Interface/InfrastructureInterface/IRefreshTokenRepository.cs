using BaseCleanWithJwt.Domain.Entities;

namespace BaseCleanWithJwt.Application.Interface.InfrastructureInterface;

public interface IRefreshTokenRepository
{
    Task<RefreshTokenModel?> GetByTokenAsync(string token);
    Task UpdateRefreshTokenAsync(Guid id, string token, DateTime expiresAt);
    Task DeleteByIdAsync(Guid id);
}