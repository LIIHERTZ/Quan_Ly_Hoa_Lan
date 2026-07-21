using MediatR;

namespace QuanLyHoaLan.Application.Features.Discussions.Commands.DeleteDiscussionPost;

public record DeleteDiscussionPostCommand(Guid Id) : IRequest<Unit>;
