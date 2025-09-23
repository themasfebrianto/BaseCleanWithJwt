using BaseCleanWithJwt.Domain.DTO.UserDTO;

namespace BaseCleanWithJwt.Application.Interface.ServiceInterface;

public interface IUserService
{
    Task<UserResponseDTO> InsertOneAsync(UserRequestDTO request);
    Task<UserResponseDTO> UpdateOneAsync(Guid id, UserRequestDTO request);
    Task<bool> DeleteOneAsync(Guid id);
    Task<UserResponseDTO?> GetByIdAsync(Guid id);
    Task<List<UserResponseDTO>> GetAllAsync();
    Task<bool> ChangePasswordAsync(string id, string oldPwd, string newPwd);
    Task<bool> SoftDeleteAsync(string id);
    Task<bool> SetRoleAsync(string id, int roleId);
    Task<bool> SetActiveAsync(string id, bool isActive);

}