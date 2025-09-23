
using BaseCleanWithJwt.Application.Interface.ServiceInterface;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;

namespace BaseCleanWithJwt.Application.Service;

public class ImageStorageService : IImageStorageService
{
    private readonly string _imageFolderPath = "wwwroot/uploads/";
    private const long _maxFileSize = 2 * 1024 * 1024; // 2MB

    public async Task<string> UploadAsync(IFormFile file, string? oldImagePath = null)
    {
        if (file.Length > _maxFileSize)
            throw new InvalidOperationException("File size exceeds 2MB limit");

        if (!Directory.Exists(_imageFolderPath))
            Directory.CreateDirectory(_imageFolderPath);

        // Delete old image if it exists
        if (!string.IsNullOrEmpty(oldImagePath))
        {
            var uri = new Uri(oldImagePath, UriKind.RelativeOrAbsolute);
            var relativePath = uri.IsAbsoluteUri ? uri.AbsolutePath : oldImagePath;
            var oldFileName = Path.GetFileName(relativePath);
            var fullOldPath = Path.Combine(_imageFolderPath, oldFileName);

            if (File.Exists(fullOldPath))
                File.Delete(fullOldPath);
        }

        var fileName = $"{Guid.NewGuid()}.webp";
        var filePath = Path.Combine(_imageFolderPath, fileName);

        // Convert to WebP
        using var image = await Image.LoadAsync<Rgba32>(file.OpenReadStream());
        var encoder = new WebpEncoder
        {
            Quality = 75 // 0-100
        };
        await image.SaveAsync(filePath, encoder);

        return $"/uploads/{fileName}";
    }
}

