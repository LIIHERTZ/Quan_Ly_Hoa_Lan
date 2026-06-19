using System.Linq.Expressions;
using MediatR;
using QuanLyHoaLan.Application.DTOs.Discussion;
using QuanLyHoaLan.Domain.Exceptions;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Discussions.Queries.GetDiscussionPostById;

public class GetDiscussionPostByIdQueryHandler : IRequestHandler<GetDiscussionPostByIdQuery, DiscussionPostDto>
{
    private readonly IBaseRepository<DiscussionPost> _postRepository;
    private readonly IBaseRepository<User> _userRepository;

    public GetDiscussionPostByIdQueryHandler(
        IBaseRepository<DiscussionPost> postRepository,
        IBaseRepository<User> userRepository)
    {
        _postRepository = postRepository;
        _userRepository = userRepository;
    }

    public async Task<DiscussionPostDto> Handle(GetDiscussionPostByIdQuery request, CancellationToken cancellationToken)
    {
        var includes = new Expression<Func<DiscussionPost, object>>[] 
        { 
            x => x.Author,
            x => x.Comments
        };

        var post = await _postRepository.FindByIdAsync(request.Id, includes);

        if (post == null)
        {
            throw new NotFoundException(nameof(DiscussionPost), request.Id);
        }

        // Fetch authors for comments manually since ThenInclude is not easily supported in this generic repo
        var commentAuthorIds = post.Comments.Select(c => c.AuthorId).Distinct().ToList();
        var authors = new Dictionary<Guid, User>();
        if (commentAuthorIds.Any())
        {
            Expression<Func<User, bool>>[] filters = new Expression<Func<User, bool>>[] { u => commentAuthorIds.Contains(u.Id) };
            var usersResult = await _userRepository.FindResultAsync(filters, null, 0, commentAuthorIds.Count, null);
            authors = usersResult.Items.ToDictionary(u => u.Id);
        }

        var comments = post.Comments
            .OrderBy(c => c.CreatedAt)
            .Select(c => new DiscussionCommentDto
            {
                Id = c.Id,
                Content = c.Content,
                AuthorId = c.AuthorId,
                AuthorName = authors.ContainsKey(c.AuthorId) ? authors[c.AuthorId].FullName : string.Empty,
                CreatedAt = c.CreatedAt
            }).ToList();

        return new DiscussionPostDto
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            AuthorId = post.AuthorId,
            AuthorName = post.Author?.FullName ?? string.Empty,
            CreatedAt = post.CreatedAt,
            CommentCount = post.Comments.Count,
            Comments = comments
        };
    }
}
