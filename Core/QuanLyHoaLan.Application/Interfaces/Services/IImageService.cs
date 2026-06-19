using System.IO;
using QuanLyHoaLan.Application.Models;

namespace QuanLyHoaLan.Application.Interfaces.Services;

public interface IImageService
{
    Task<ImageUploadResult?> UploadImageAsync(Stream fileStream, string fileName);
    Task<bool> DeleteImageAsync(string publicId);
}
