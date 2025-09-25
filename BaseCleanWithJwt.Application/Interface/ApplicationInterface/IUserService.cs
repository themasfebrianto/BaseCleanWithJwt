using BaseCleanWithJwt.Domain.DTO.UserDTO;

namespace BaseCleanWithJwt.Application.Interface.ApplicationInterface;

public interface IUserService
{
    Task<UserResponseDTO> InsertOneAsync(UserRequestDTO request);
    Task<UserResponseDTO> UpdateOneAsync(UserRequestDTO request);
    Task<bool> DeleteOneAsync(Guid id);
    Task<UserResponseDTO?> GetByIdAsync(Guid id);
    Task<UserResponseDTO?> GetByEmailAsync(string email);
    Task<List<UserResponseDTO>> GetAllAsync();
    Task<bool> ChangePasswordAsync(Guid id, string oldPwd, string newPwd);
    Task<bool> SoftDeleteAsync(Guid id);
    Task<bool> SetRoleAsync(Guid id, int roleId);
    Task<bool> SetActiveAsync(Guid id, bool isActive);

}