using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyHoaLan.Application.Features.Auth.Commands.LoginWithGoogle;
using QuanLyHoaLan.Application.Features.Auth.Commands.Login;
using QuanLyHoaLan.Application.Features.Auth.Commands.Register;

namespace QuanLyHoaLan.API.Controllers;

public class AuthController : BaseController
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResult(result);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResult(result);
    }

    [AllowAnonymous]

    [HttpPost("google-login")]
    public async Task<IActionResult> GoogleLogin([FromBody] LoginWithGoogleCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResult(result);
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] QuanLyHoaLan.Application.Features.Auth.Commands.RefreshToken.RefreshTokenCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResult(result);
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] QuanLyHoaLan.Application.Features.Auth.Commands.Logout.LogoutCommand command)
    {
        await Mediator.Send(command);
        return OkResult("Đăng xuất thành công");
    }
}
