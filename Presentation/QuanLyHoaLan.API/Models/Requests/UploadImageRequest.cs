using Microsoft.AspNetCore.Http;

namespace QuanLyHoaLan.API.Models.Requests;

public class UploadImageRequest
{
    public IFormFile File { get; set; } = null!;
}
