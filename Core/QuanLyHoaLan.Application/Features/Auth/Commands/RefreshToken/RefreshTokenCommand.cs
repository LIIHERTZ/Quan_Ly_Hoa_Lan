using MediatR;
using QuanLyHoaLan.Application.DTOs;

namespace QuanLyHoaLan.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommand : IRequest<AuthResultDto>
{
    public string Token { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}
