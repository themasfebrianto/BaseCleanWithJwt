

using BaseCleanWithJwt.Domain.DTO.UserDTO;

namespace BaseCleanWithJwt.Application.Interface.RepositoryInterface;

public interface IUserRepository
{
    Task<UserResponseDTO?> GetByEmailAsync(string username);
    Task<UserResponseDTO?> GetByIdAsync(string email);
    Task<List<UserResponseDTO>> GetAllAsync(UserFilterDTO filter);
    Task<UserResponseDTO> InsertOneAsync(UserResponseDTO request);
    Task<UserResponseDTO> UpdateOneAsync(UserResponseDTO user);
}