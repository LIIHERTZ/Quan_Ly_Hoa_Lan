using MediatR;

namespace QuanLyHoaLan.Application.Features.Discussions.Commands.DeleteDiscussionComment;

public record DeleteDiscussionCommentCommand(Guid PostId, Guid CommentId) : IRequest<Unit>;
