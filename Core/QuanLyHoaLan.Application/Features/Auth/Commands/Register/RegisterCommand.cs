using MediatR;
using QuanLyHoaLan.Application.DTOs;

namespace QuanLyHoaLan.Application.Features.Auth.Commands.Register;

// Command for user registration
public class RegisterCommand : IRequest<AuthResultDto>
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}
