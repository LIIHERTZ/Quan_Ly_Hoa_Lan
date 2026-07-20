using System.Linq.Expressions;
using MediatR;
using QuanLyHoaLan.Application.Common.Models;
using QuanLyHoaLan.Application.DTOs.User;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Users.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PaginatedList<UserDto>>
{
    private readonly IBaseRepository<User> _userRepository;

    public GetUsersQueryHandler(IBaseRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<PaginatedList<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var filters = new List<Expression<Func<User, bool>>>();
        var searchTerm = request.SearchTerm?.Trim().ToLowerInvariant();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            filters.Add(user => user.Email.ToLower().Contains(searchTerm)
                || user.FullName.ToLower().Contains(searchTerm));
        }

        if (request.RoleId.HasValue)
        {
            filters.Add(user => user.RoleId == request.RoleId.Value);
        }

        var skip = (request.PageNumber - 1) * request.PageSize;
        var sortBy = request.SortBy?.Trim().ToLowerInvariant() switch
        {
            "email" => "Email",
            "fullname" => "FullName",
            "updatedat" => "UpdatedAt",
            _ => "CreatedAt"
        };
        var orderBy = request.SortDescending ? $"{sortBy} desc" : sortBy;
        var result = await _userRepository.FindResultAsync(
            filters.Count == 0 ? null : filters.ToArray(),
            orderBy,
            skip,
            request.PageSize,
            [user => user.Role!]);

        var users = result.Items.Select(user => user.ToDto()).ToList();
        return PaginatedList<UserDto>.Create(users, result.TotalCount, request.PageNumber, request.PageSize);
    }
}
