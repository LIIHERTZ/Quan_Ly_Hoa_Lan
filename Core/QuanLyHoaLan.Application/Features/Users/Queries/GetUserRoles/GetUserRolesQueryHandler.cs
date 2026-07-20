using System.Linq.Expressions;
using MediatR;
using QuanLyHoaLan.Application.DTOs.User;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Users.Queries.GetUserRoles;

public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, IReadOnlyCollection<RoleOptionDto>>
{
    private readonly IBaseRepository<Role> _roleRepository;

    public GetUserRolesQueryHandler(IBaseRepository<Role> roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<IReadOnlyCollection<RoleOptionDto>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Role, bool>>[] filters = [role => role.IsActive];
        var roles = await _roleRepository.FindAsync(filters, "Name", 0, 0);

        return roles.Select(role => new RoleOptionDto
        {
            Id = role.Id,
            Code = role.Code,
            Name = role.Name
        }).ToList();
    }
}
