using System.Linq.Expressions;
using MediatR;
using QuanLyHoaLan.Application.DTOs.User;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Exceptions;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
{
    private readonly IBaseRepository<User> _userRepository;
    private readonly IBaseRepository<JwtRefreshToken> _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserCommandHandler(
        IBaseRepository<User> userRepository,
        IBaseRepository<JwtRefreshToken> refreshTokenRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
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

        var emailChanged = !string.Equals(user.Email, email, StringComparison.Ordinal);
        user.Email = email;
        user.FullName = request.FullName.Trim();
        user.AvatarUrl = string.IsNullOrWhiteSpace(request.AvatarUrl) ? null : request.AvatarUrl.Trim();

        await _userRepository.UpdateAsync(user, cancellationToken);
        if (emailChanged)
        {
            await UserTokenRevocation.RevokeAllAsync(_refreshTokenRepository, user.Id, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return user.ToDto();
    }
}
