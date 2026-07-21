using QuanLyHoaLan.Domain.Common;
using QuanLyHoaLan.Domain.Enums;

namespace QuanLyHoaLan.Domain.Entities;

public class ArticleCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public ArticleCategoryType Type { get; set; }

    public Guid? ParentId { get; set; }
    public virtual ArticleCategory? ParentCategory { get; set; }
    public virtual ICollection<ArticleCategory> SubCategories { get; set; } = new List<ArticleCategory>();

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
}
