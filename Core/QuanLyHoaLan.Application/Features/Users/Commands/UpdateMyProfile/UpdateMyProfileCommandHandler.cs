using MediatR;
using QuanLyHoaLan.Application.DTOs.User;
using QuanLyHoaLan.Application.Interfaces;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Exceptions;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Users.Commands.UpdateMyProfile;

public class UpdateMyProfileCommandHandler
    : IRequestHandler<UpdateMyProfileCommand, UserDto>
{
    private readonly IBaseRepository<User> _userRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMyProfileCommandHandler(
        IBaseRepository<User> userRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserDto> Handle(
        UpdateMyProfileCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindByIdAsync(
            _currentUser.UserId,
            entity => entity.Role!);

        if (user == null)
        {
            throw new NotFoundException(nameof(User), _currentUser.UserId);
        }

        user.FullName = request.FullName.Trim();
        user.AvatarUrl = string.IsNullOrWhiteSpace(request.AvatarUrl)
            ? null
            : request.AvatarUrl.Trim();

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.ToDto();
    }
}
