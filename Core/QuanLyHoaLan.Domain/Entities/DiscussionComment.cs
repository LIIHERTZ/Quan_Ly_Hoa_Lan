using QuanLyHoaLan.Domain.Common;

namespace QuanLyHoaLan.Domain.Entities;

public class DiscussionComment : BaseEntity
{
    public string Content { get; set; } = string.Empty;

    public Guid PostId { get; set; }
    public virtual DiscussionPost Post { get; set; } = null!;

    public Guid AuthorId { get; set; }
    public virtual User Author { get; set; } = null!;
}
