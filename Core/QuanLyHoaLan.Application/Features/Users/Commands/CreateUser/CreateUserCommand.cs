using MediatR;
using QuanLyHoaLan.Application.DTOs.User;

namespace QuanLyHoaLan.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommand : IRequest<UserDto>
{
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
}
