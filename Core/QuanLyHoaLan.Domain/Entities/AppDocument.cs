using QuanLyHoaLan.Domain.Common;

namespace QuanLyHoaLan.Domain.Entities;

public class AppDocument : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string OriginalName { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? PublicId { get; set; }

    public Guid? CategoryId { get; set; }
    public virtual DocumentCategory? Category { get; set; }
}
