using MediatR;
using QuanLyHoaLan.Domain.Exceptions;
using QuanLyHoaLan.Application.Interfaces;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Discussions.Commands.CreateDiscussionComment;

public class CreateDiscussionCommentCommandHandler : IRequestHandler<CreateDiscussionCommentCommand, Guid>
{
    private readonly IBaseRepository<DiscussionComment> _commentRepository;
    private readonly IBaseRepository<DiscussionPost> _postRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDiscussionCommentCommandHandler(
        IBaseRepository<DiscussionComment> commentRepository,
        IBaseRepository<DiscussionPost> postRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _commentRepository = commentRepository;
        _postRepository = postRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateDiscussionCommentCommand request, CancellationToken cancellationToken)
    {
        var postExists = await _postRepository.ExistsAsync(request.PostId);
        if (!postExists)
        {
            throw new NotFoundException(nameof(DiscussionPost), request.PostId);
        }

        var comment = new DiscussionComment
        {
            PostId = request.PostId,
            Content = request.Content,
            AuthorId = _currentUser.UserId
        };

        await _commentRepository.InsertAsync(comment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return comment.Id;
    }
}
