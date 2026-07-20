using MediatR;
using QuanLyHoaLan.Application.DTOs.User;

namespace QuanLyHoaLan.Application.Features.Users.Queries.GetUserRoles;

public record GetUserRolesQuery : IRequest<IReadOnlyCollection<RoleOptionDto>>;
