

using BaseCleanWithJwt.Domain.DTO.UserDTO;
using BaseCleanWithJwt.Domain.Entities;

namespace BaseCleanWithJwt.Application.Interface.RepositoryInterface;

public interface IUserRepository
{
    Task<UserModel?> GetByEmailAsync(string username);
    Task<UserModel?> GetByIdAsync(Guid id);
    Task<List<UserModel>> GetAllAsync();
    Task<UserModel> InsertOneAsync(UserModel request);
    Task<UserModel> UpdateOneAsync(UserModel user);
    Task DeleteOneAsync(Guid id);
}