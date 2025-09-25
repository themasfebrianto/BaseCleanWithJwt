using Microsoft.AspNetCore.Http;

namespace BaseCleanWithJwt.Application.Interface.InfrastructureInterface;

public interface IImageStorageService
{
    Task<string> UploadAsync(IFormFile file, string? oldImagePath = null);
}
