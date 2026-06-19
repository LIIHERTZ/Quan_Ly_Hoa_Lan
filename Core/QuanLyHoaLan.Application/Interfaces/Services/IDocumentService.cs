using System.IO;
using QuanLyHoaLan.Application.Models;

namespace QuanLyHoaLan.Application.Interfaces.Services;

public interface IDocumentService
{
    Task<DocumentUploadResult?> UploadDocumentAsync(Stream fileStream, string fileName);
    Task<bool> DeleteDocumentAsync(string publicId);
}
