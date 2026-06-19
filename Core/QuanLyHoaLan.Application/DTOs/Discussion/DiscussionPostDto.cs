namespace QuanLyHoaLan.Application.DTOs.Discussion;

public class DiscussionPostDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public int CommentCount { get; set; }
    
    // Detailed view only
    public List<DiscussionCommentDto>? Comments { get; set; }
}
