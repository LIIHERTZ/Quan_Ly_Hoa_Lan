using System.Linq.Expressions;
using MediatR;
using QuanLyHoaLan.Application.DTOs.User;
using QuanLyHoaLan.Application.Interfaces;
using QuanLyHoaLan.Domain.Constants;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Exceptions;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
{
    private readonly IBaseRepository<User> _userRepository;
    private readonly IBaseRepository<Role> _roleRepository;
    private readonly IBaseRepository<JwtRefreshToken> _refreshTokenRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserCommandHandler(
        IBaseRepository<User> userRepository,
        IBaseRepository<Role> roleRepository,
        IBaseRepository<JwtRefreshToken> refreshTokenRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindByIdAsync(request.Id, entity => entity.Role!);
        if (user == null)
        {
            throw new NotFoundException(nameof(User), request.Id);
        }

        var email = request.Email.Trim().ToLowerInvariant();
        Expression<Func<User, bool>>[] emailFilters =
        [
            entity => entity.Id != request.Id && entity.Email.ToLower() == email
        ];
        if (await _userRepository.AnyAsync(emailFilters, includeDeleted: true))
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                [nameof(request.Email)] = ["Email đã được sử dụng, kể cả bởi tài khoản đã xóa."]
            });
        }

        Expression<Func<Role, bool>>[] roleFilters =
        [
            role => role.Id == request.RoleId && role.IsActive
        ];
        var newRole = await _roleRepository.FindOneAsync(roleFilters);
        if (newRole == null)
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                [nameof(request.RoleId)] = ["Vai trò không tồn tại hoặc đã bị vô hiệu hóa."]
            });
        }

        var roleChanged = user.RoleId != newRole.Id;
        if (user.Id == _currentUser.UserId && roleChanged)
        {
            throw new InvalidOperationException("Bạn không thể tự thay đổi vai trò của chính mình.");
        }

        if (roleChanged
            && user.Role?.Code == RoleConstants.AdminCode
            && newRole.Code != RoleConstants.AdminCode)
        {
            Expression<Func<User, bool>>[] adminFilters =
            [
                entity => entity.Role != null && entity.Role.Code == RoleConstants.AdminCode
            ];
            if (await _userRepository.CountAsync(adminFilters) <= 1)
            {
                throw new InvalidOperationException("Không thể hạ quyền Admin cuối cùng của hệ thống.");
            }
        }

        var securityInformationChanged = roleChanged || !string.Equals(user.Email, email, StringComparison.Ordinal);
        user.Email = email;
        user.FullName = request.FullName.Trim();
        user.RoleId = newRole.Id;
        user.Role = newRole;
        user.AvatarUrl = string.IsNullOrWhiteSpace(request.AvatarUrl) ? null : request.AvatarUrl.Trim();

        await _userRepository.UpdateAsync(user, cancellationToken);
        if (securityInformationChanged)
        {
            await UserTokenRevocation.RevokeAllAsync(_refreshTokenRepository, user.Id, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return user.ToDto();
    }
}
