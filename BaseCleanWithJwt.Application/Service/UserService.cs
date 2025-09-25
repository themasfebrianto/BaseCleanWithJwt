using BaseCleanWithJwt.Application.Interface.ApplicationInterface;
using BaseCleanWithJwt.Application.Interface.InfrastructureInterface;
using BaseCleanWithJwt.Application.Mapping;
using BaseCleanWithJwt.Domain.DTO.UserDTO;

namespace BaseCleanWithJwt.Application.Service;


public class UserService(
    IUserRepository repository,
    IImageStorageService imageStorageService
) : IUserService
{
    public async Task<bool> ChangePasswordAsync(Guid id, string oldPwd, string newPwd)
    {
        var user = await repository.GetByIdAsync(id);
        if (user is null) return false;

        if (!BCrypt.Net.BCrypt.Verify(oldPwd, user.PasswordHash))
            return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPwd);
        user.UpdatedAt = DateTime.UtcNow;

        await repository.UpdateOneAsync(user);
        return true;
    }

    public async Task<UserResponseDTO> InsertOneAsync(UserRequestDTO request)
    {
        var user = request.MapToUserModel();

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        user.IsActive = true;

        if (request.AvatarFile != null)
            user.AvatarUrl = await imageStorageService.UploadAsync(request.AvatarFile);

        var isUserCreated = await repository.InsertOneAsync(user);
        var userRes = await repository.GetByEmailAsync(request.Email) ?? throw new Exception("User creation failed");
        return userRes.MapToUserResponseDTO();
    }

    public async Task<List<UserResponseDTO>> GetAllAsync()
    {
        var users = await repository.GetAllAsync();
        return users.Select(u => u.MapToUserResponseDTO()).ToList()!;
    }

    public async Task<UserResponseDTO?> GetByEmailAsync(string email)
    {
        var user = await repository.GetByEmailAsync(email);
        return user!.MapToUserResponseDTO() ?? throw new Exception("User not found");
    }

    public async Task<UserResponseDTO?> GetByIdAsync(Guid id)
    {
        var user = await repository.GetByIdAsync(id);
        return user?.MapToUserResponseDTO();
    }

    public async Task<bool> SetActiveAsync(Guid id, bool isActive)
    {
        var user = await repository.GetByIdAsync(id);
        if (user is null) return false;

        user.IsActive = isActive;
        user.UpdatedAt = DateTime.UtcNow;
        await repository.UpdateOneAsync(user);
        return true;
    }

    public async Task<bool> SetRoleAsync(Guid id, int roleId)
    {
        var user = await repository.GetByIdAsync(id);
        if (user is null) return false;

        user.RoleId = roleId;
        user.UpdatedAt = DateTime.UtcNow;
        await repository.UpdateOneAsync(user);
        return true;
    }

    public async Task<bool> SoftDeleteAsync(Guid id)
    {
        var user = await repository.GetByIdAsync(id);
        if (user is null) return false;

        user.DeletedAt = DateTime.UtcNow;
        await repository.UpdateOneAsync(user);
        return true;
    }

    public async Task<UserResponseDTO> UpdateOneAsync(UserRequestDTO request)
    {
        string? avatarUrl = null;
        if (request.AvatarFile != null)
            avatarUrl = await imageStorageService.UploadAsync(request.AvatarFile);

        var userModel = request.MapToUserModel();

        if (!string.IsNullOrEmpty(avatarUrl))
            userModel.AvatarUrl = avatarUrl;

        userModel.UpdatedAt = DateTime.UtcNow;
        var user = await repository.UpdateOneAsync(userModel);
        return user.MapToUserResponseDTO();
    }

    public Task<UserResponseDTO> UpdateOneAsync(Guid id, UserRequestDTO request)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteOneAsync(Guid id)
    {
        var user = await repository.GetByIdAsync(id);
        if (user is null) return false;

        await repository.DeleteOneAsync(id);
        return true;
    }

}

