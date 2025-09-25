using BaseCleanWithJwt.Domain.Entities;

namespace BaseCleanWithJwt.Application.Interface.InfrastructureInterface;


public interface IJwtProvider
{
    string GenerateToken(UserModel UserModel);
    string GenerateRefreshToken();
}