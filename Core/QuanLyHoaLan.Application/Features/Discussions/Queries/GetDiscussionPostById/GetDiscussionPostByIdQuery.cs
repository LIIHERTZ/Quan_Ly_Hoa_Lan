using MediatR;
using QuanLyHoaLan.Application.DTOs.Discussion;

namespace QuanLyHoaLan.Application.Features.Discussions.Queries.GetDiscussionPostById;

public class GetDiscussionPostByIdQuery : IRequest<DiscussionPostDto>
{
    public Guid Id { get; set; }
}
