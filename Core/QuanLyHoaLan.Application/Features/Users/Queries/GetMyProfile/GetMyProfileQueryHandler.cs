using MediatR;
using QuanLyHoaLan.Application.DTOs.User;
using QuanLyHoaLan.Application.Interfaces;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Exceptions;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Users.Queries.GetMyProfile;

public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, UserDto>
{
    private readonly IBaseRepository<User> _userRepository;
    private readonly ICurrentUser _currentUser;

    public GetMyProfileQueryHandler(
        IBaseRepository<User> userRepository,
        ICurrentUser currentUser)
    {
        _userRepository = userRepository;
        _currentUser = currentUser;
    }

    public async Task<UserDto> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindByIdAsync(
            _currentUser.UserId,
            entity => entity.Role!);

        if (user == null)
        {
            throw new NotFoundException(nameof(User), _currentUser.UserId);
        }

        return user.ToDto();
    }
}
