namespace QuanLyHoaLan.Application.DTOs.Discussion;

public class DiscussionCommentDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}
