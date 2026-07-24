namespace QuanLyHoaLan.Application.DTOs.DocumentCategory;

public class DocumentCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public string ParentName { get; set; } = string.Empty;
    public int DocumentCount { get; set; }
    public List<DocumentCategoryDto> SubCategories { get; set; } = new();
}
