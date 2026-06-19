namespace QuanLyHoaLan.Application.DTOs.Document;

public class AppDocumentDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string OriginalName { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string Url { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}
