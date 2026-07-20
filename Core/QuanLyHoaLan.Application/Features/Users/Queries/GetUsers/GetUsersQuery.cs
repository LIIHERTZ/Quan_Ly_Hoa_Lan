using MediatR;
using QuanLyHoaLan.Application.Common.Models;
using QuanLyHoaLan.Application.DTOs.User;

namespace QuanLyHoaLan.Application.Features.Users.Queries.GetUsers;

public class GetUsersQuery : PagedRequest, IRequest<PaginatedList<UserDto>>
{
    public Guid? RoleId { get; set; }

    public GetUsersQuery()
    {
        SortDescending = true;
    }
}
