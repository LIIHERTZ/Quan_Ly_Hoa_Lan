using QuanLyHoaLan.Domain.Common;

namespace QuanLyHoaLan.Domain.Entities;

public class Article : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    
    public Guid? ThumbnailImageId { get; set; }
    
    public Guid AuthorId { get; set; }
    public virtual User Author { get; set; } = null!;

    public bool IsPublished { get; set; }
    public DateTimeOffset? PublishedAt { get; set; }

    public virtual ICollection<ArticleCategory> Categories { get; set; } = new List<ArticleCategory>();

    // Array of Orchid IDs
    public List<Guid> OrchidIds { get; set; } = new();

    // PostgreSQL Array Pattern for Documents
    public List<Guid> DocumentIds { get; set; } = new();
}
