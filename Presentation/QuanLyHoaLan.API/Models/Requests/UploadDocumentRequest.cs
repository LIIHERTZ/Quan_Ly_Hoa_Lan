using Microsoft.AspNetCore.Http;

namespace QuanLyHoaLan.API.Models.Requests;

public class UploadDocumentRequest
{
    public IFormFile File { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}
