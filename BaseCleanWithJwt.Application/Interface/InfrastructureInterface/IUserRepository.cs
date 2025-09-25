using BaseCleanWithJwt.Domain.Entities;

namespace BaseCleanWithJwt.Application.Interface.InfrastructureInterface;

public interface IUserRepository
{
    Task<UserModel?> GetByEmailAsync(string username);
    Task<UserModel?> GetByIdAsync(Guid id);
    Task<List<UserModel>> GetAllAsync();
    Task<bool> InsertOneAsync(UserModel request);
    Task<UserModel> UpdateOneAsync(UserModel user);
    Task DeleteOneAsync(Guid id);
}