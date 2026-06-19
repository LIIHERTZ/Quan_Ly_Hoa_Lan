using QuanLyHoaLan.Domain.Common;

namespace QuanLyHoaLan.Domain.Entities;

public class UploadedImage : BaseEntity
{
    public string Url { get; set; } = string.Empty;
    public string PublicId { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
}
