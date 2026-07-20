using MediatR;

namespace QuanLyHoaLan.Application.Features.Users.Commands.ResetUserPassword;

public class ResetUserPasswordCommand : IRequest
{
    public Guid Id { get; set; }
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}
