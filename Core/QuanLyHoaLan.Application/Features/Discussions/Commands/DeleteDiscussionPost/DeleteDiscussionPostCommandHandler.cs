using System.Linq.Expressions;
using MediatR;
using QuanLyHoaLan.Application.Interfaces;
using QuanLyHoaLan.Domain.Constants;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Exceptions;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Discussions.Commands.DeleteDiscussionPost;

public class DeleteDiscussionPostCommandHandler : IRequestHandler<DeleteDiscussionPostCommand, Unit>
{
    private readonly IBaseRepository<DiscussionPost> _postRepository;
    private readonly IBaseRepository<DiscussionComment> _commentRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDiscussionPostCommandHandler(
        IBaseRepository<DiscussionPost> postRepository,
        IBaseRepository<DiscussionComment> commentRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _postRepository = postRepository;
        _commentRepository = commentRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteDiscussionPostCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.FindByIdAsync(request.Id);
        if (post == null)
        {
            throw new NotFoundException(nameof(DiscussionPost), request.Id);
        }

        var isOwner = post.AuthorId == _currentUser.UserId;
        var isAdmin = _currentUser.HasRole(RoleConstants.Admin);
        if (!isOwner && !isAdmin)
        {
            throw new ForbiddenAccessException("Bạn chỉ có thể xóa bài thảo luận của chính mình.");
        }

        Expression<Func<DiscussionComment, bool>>[] commentFilters =
            [comment => comment.PostId == post.Id];
        var comments = await _commentRepository.FindAsync(filters: commentFilters);

        if (comments.Count > 0)
        {
            await _commentRepository.DeleteRangeAsync(comments, cancellationToken);
        }

        await _postRepository.DeleteAsync(post, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
