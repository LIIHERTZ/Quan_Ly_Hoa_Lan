using MediatR;

namespace QuanLyHoaLan.Application.Features.Discussions.Commands.CreateDiscussionPost;

public class CreateDiscussionPostCommand : IRequest<Guid>
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
