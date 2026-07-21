using MediatR;
using QuanLyHoaLan.Application.Interfaces;
using QuanLyHoaLan.Domain.Constants;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Exceptions;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Discussions.Commands.DeleteDiscussionComment;

public class DeleteDiscussionCommentCommandHandler
    : IRequestHandler<DeleteDiscussionCommentCommand, Unit>
{
    private readonly IBaseRepository<DiscussionComment> _commentRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDiscussionCommentCommandHandler(
        IBaseRepository<DiscussionComment> commentRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _commentRepository = commentRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(
        DeleteDiscussionCommentCommand request,
        CancellationToken cancellationToken)
    {
        var comment = await _commentRepository.FindByIdAsync(request.CommentId);
        if (comment == null || comment.PostId != request.PostId)
        {
            throw new NotFoundException(nameof(DiscussionComment), request.CommentId);
        }

        var isOwner = comment.AuthorId == _currentUser.UserId;
        var isAdmin = _currentUser.HasRole(RoleConstants.Admin);
        if (!isOwner && !isAdmin)
        {
            throw new ForbiddenAccessException("Bạn chỉ có thể xóa bình luận của chính mình.");
        }

        await _commentRepository.DeleteAsync(comment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
