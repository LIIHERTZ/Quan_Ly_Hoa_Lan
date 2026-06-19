using MediatR;

namespace QuanLyHoaLan.Application.Features.Discussions.Commands.CreateDiscussionComment;

public class CreateDiscussionCommentCommand : IRequest<Guid>
{
    public Guid PostId { get; set; }
    public string Content { get; set; } = string.Empty;
}
