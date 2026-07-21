namespace QuanLyHoaLan.Application.DTOs.ArticleCategory;

using QuanLyHoaLan.Domain.Enums;

public class ArticleCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public ArticleCategoryType Type { get; set; }
    public Guid? ParentId { get; set; }
    public string ParentName { get; set; } = string.Empty;
    public List<ArticleCategoryDto> SubCategories { get; set; } = new();
}
