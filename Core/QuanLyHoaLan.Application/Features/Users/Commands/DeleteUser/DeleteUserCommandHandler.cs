using System.Linq.Expressions;
using MediatR;
using QuanLyHoaLan.Application.Interfaces;
using QuanLyHoaLan.Domain.Constants;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Exceptions;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly IBaseRepository<User> _userRepository;
    private readonly IBaseRepository<JwtRefreshToken> _refreshTokenRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserCommandHandler(
        IBaseRepository<User> userRepository,
        IBaseRepository<JwtRefreshToken> refreshTokenRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindByIdAsync(request.Id, entity => entity.Role!);
        if (user == null)
        {
            throw new NotFoundException(nameof(User), request.Id);
        }

        if (user.Id == _currentUser.UserId)
        {
            throw new InvalidOperationException("Bạn không thể tự xóa tài khoản đang đăng nhập.");
        }

        if (user.Role?.Code == RoleConstants.AdminCode)
        {
            Expression<Func<User, bool>>[] adminFilters =
            [
                entity => entity.Role != null && entity.Role.Code == RoleConstants.AdminCode
            ];
            if (await _userRepository.CountAsync(adminFilters) <= 1)
            {
                throw new InvalidOperationException("Không thể xóa Admin cuối cùng của hệ thống.");
            }
        }

        await UserTokenRevocation.RevokeAllAsync(_refreshTokenRepository, user.Id, cancellationToken);
        await _userRepository.DeleteAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
