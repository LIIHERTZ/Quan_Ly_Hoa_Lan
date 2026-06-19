using System.IO;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using QuanLyHoaLan.Application.Interfaces.Services;
using AppImageUploadResult = QuanLyHoaLan.Application.Models.ImageUploadResult;
using QuanLyHoaLan.Infrastructure.Settings;

namespace QuanLyHoaLan.Infrastructure.Services;

public class CloudinaryService : IImageService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IOptions<CloudinarySettings> config)
    {
        var acc = new Account(
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
        );

        _cloudinary = new Cloudinary(acc);
    }

    public async Task<AppImageUploadResult?> UploadImageAsync(Stream fileStream, string fileName)
    {
        var uploadResult = new AppImageUploadResult();

        if (fileStream.Length > 0)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, fileStream),
                Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
            {
                return null;
            }

            uploadResult.Url = result.SecureUrl.ToString();
            uploadResult.PublicId = result.PublicId;
            return uploadResult;
        }

        return null;
    }

    public async Task<bool> DeleteImageAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deleteParams);

        return result.Result == "ok";
    }
}
