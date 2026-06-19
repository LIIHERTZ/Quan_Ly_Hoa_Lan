using MediatR;
using QuanLyHoaLan.Application.Interfaces;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Discussions.Commands.CreateDiscussionPost;

public class CreateDiscussionPostCommandHandler : IRequestHandler<CreateDiscussionPostCommand, Guid>
{
    private readonly IBaseRepository<DiscussionPost> _postRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDiscussionPostCommandHandler(
        IBaseRepository<DiscussionPost> postRepository, 
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _postRepository = postRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateDiscussionPostCommand request, CancellationToken cancellationToken)
    {
        var post = new DiscussionPost
        {
            Title = request.Title,
            Content = request.Content,
            AuthorId = _currentUser.UserId
        };

        await _postRepository.InsertAsync(post, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return post.Id;
    }
}
