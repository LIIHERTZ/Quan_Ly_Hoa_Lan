using MediatR;
using QuanLyHoaLan.Application.DTOs.User;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Exceptions;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
{
    private readonly IBaseRepository<User> _userRepository;

    public GetUserByIdQueryHandler(IBaseRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindByIdAsync(request.Id, entity => entity.Role!);
        if (user == null)
        {
            throw new NotFoundException(nameof(User), request.Id);
        }

        return user.ToDto();
    }
}
