using System.Linq.Expressions;
using MediatR;
using QuanLyHoaLan.Application.DTOs.User;
using QuanLyHoaLan.Domain.Constants;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Exceptions;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IBaseRepository<User> _userRepository;
    private readonly IBaseRepository<Role> _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(
        IBaseRepository<User> userRepository,
        IBaseRepository<Role> roleRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        Expression<Func<User, bool>>[] emailFilters = [user => user.Email.ToLower() == email];
        if (await _userRepository.AnyAsync(emailFilters, includeDeleted: true))
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                [nameof(request.Email)] = ["Email đã được sử dụng, kể cả bởi tài khoản đã xóa."]
            });
        }

        Expression<Func<Role, bool>>[] roleFilters =
        [
            role => role.Code == RoleConstants.UserCode && role.IsActive
        ];
        var role = await _roleRepository.FindOneAsync(roleFilters);
        if (role == null)
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                ["Role"] = ["Vai trò USER mặc định không tồn tại hoặc đã bị vô hiệu hóa."]
            });
        }

        var user = new User
        {
            Email = email,
            FullName = request.FullName.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            RoleId = role.Id,
            Role = role,
            AvatarUrl = string.IsNullOrWhiteSpace(request.AvatarUrl) ? null : request.AvatarUrl.Trim()
        };

        await _userRepository.InsertAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return user.ToDto();
    }
}
