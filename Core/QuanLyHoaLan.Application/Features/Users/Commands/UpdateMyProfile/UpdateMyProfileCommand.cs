using MediatR;
using QuanLyHoaLan.Application.DTOs.User;

namespace QuanLyHoaLan.Application.Features.Users.Commands.UpdateMyProfile;

public class UpdateMyProfileCommand : IRequest<UserDto>
{
    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
}
