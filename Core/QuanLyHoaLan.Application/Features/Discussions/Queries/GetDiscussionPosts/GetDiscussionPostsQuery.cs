using MediatR;
using QuanLyHoaLan.Application.DTOs.Discussion;
using QuanLyHoaLan.Application.Common.Models;

namespace QuanLyHoaLan.Application.Features.Discussions.Queries.GetDiscussionPosts;

public class GetDiscussionPostsQuery : IRequest<PaginatedList<DiscussionPostDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
}
