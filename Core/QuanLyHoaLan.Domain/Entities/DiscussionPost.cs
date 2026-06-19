using QuanLyHoaLan.Domain.Common;

namespace QuanLyHoaLan.Domain.Entities;

public class DiscussionPost : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    public Guid AuthorId { get; set; }
    public virtual User Author { get; set; } = null!;

    public virtual ICollection<DiscussionComment> Comments { get; set; } = new List<DiscussionComment>();
}
