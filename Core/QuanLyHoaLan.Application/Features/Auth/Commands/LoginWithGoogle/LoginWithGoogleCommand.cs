using MediatR;
using QuanLyHoaLan.Application.DTOs;

namespace QuanLyHoaLan.Application.Features.Auth.Commands.LoginWithGoogle;

public class LoginWithGoogleCommand : IRequest<AuthResultDto>
{
    public string IdToken { get; set; } = string.Empty;
}
