using System.Linq.Expressions;
using MediatR;
using QuanLyHoaLan.Application.DTOs.Discussion;
using QuanLyHoaLan.Application.Common.Models;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Discussions.Queries.GetDiscussionPosts;

public class GetDiscussionPostsQueryHandler : IRequestHandler<GetDiscussionPostsQuery, PaginatedList<DiscussionPostDto>>
{
    private readonly IBaseRepository<DiscussionPost> _postRepository;

    public GetDiscussionPostsQueryHandler(IBaseRepository<DiscussionPost> postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<PaginatedList<DiscussionPostDto>> Handle(GetDiscussionPostsQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<DiscussionPost, bool>>[]? filters = null;
        var searchLower = request.SearchTerm?.ToLower();

        if (!string.IsNullOrEmpty(searchLower))
        {
            filters = new Expression<Func<DiscussionPost, bool>>[]
            {
                x => x.Title.ToLower().Contains(searchLower) || x.Content.ToLower().Contains(searchLower)
            };
        }

        var skip = (request.PageNumber - 1) * request.PageSize;

        var includes = new Expression<Func<DiscussionPost, object>>[] 
        { 
            x => x.Author,
            x => x.Comments
        };

        var result = await _postRepository.FindResultAsync(
            filters: filters,
            orderBy: "CreatedAt descending",
            skip: skip,
            limit: request.PageSize,
            includes: includes
        );

        var dtos = result.Items.Select(post => new DiscussionPostDto
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            AuthorId = post.AuthorId,
            AuthorName = post.Author?.FullName ?? string.Empty,
            CreatedAt = post.CreatedAt,
            CommentCount = post.Comments.Count
        }).ToList();

        return PaginatedList<DiscussionPostDto>.Create(dtos, result.TotalCount, request.PageNumber, request.PageSize);
    }
}
