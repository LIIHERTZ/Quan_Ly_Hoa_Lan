using QuanLyHoaLan.Domain.Common;

namespace QuanLyHoaLan.Domain.Entities;

public class DocumentCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    public Guid? ParentId { get; set; }
    public virtual DocumentCategory? ParentCategory { get; set; }
    public virtual ICollection<DocumentCategory> SubCategories { get; set; } = new List<DocumentCategory>();

    public virtual ICollection<AppDocument> Documents { get; set; } = new List<AppDocument>();
}
