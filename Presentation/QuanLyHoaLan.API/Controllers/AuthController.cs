using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyHoaLan.Application.Features.Auth.Commands.LoginWithGoogle;
using QuanLyHoaLan.Application.Features.Auth.Commands.Login;
using Asp.Versioning;

namespace QuanLyHoaLan.API.Controllers;

[ApiVersion("1.0")]
public class AuthController : BaseController
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        try
        {
            var result = await Mediator.Send(command);
            return OkResult(result);
        }
        catch (Exception ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [AllowAnonymous]
    [HttpPost("google-login")]
    public async Task<IActionResult> GoogleLogin([FromBody] LoginWithGoogleCommand command)
    {
        try
        {
            var result = await Mediator.Send(command);
            return OkResult(result);
        }
        catch (Exception ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] QuanLyHoaLan.Application.Features.Auth.Commands.RefreshToken.RefreshTokenCommand command)
    {
        try
        {
            var result = await Mediator.Send(command);
            return OkResult(result);
        }
        catch (Exception ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}
