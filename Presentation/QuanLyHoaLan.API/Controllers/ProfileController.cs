using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyHoaLan.Application.Features.Users.Commands.UpdateMyProfile;
using QuanLyHoaLan.Application.Features.Users.Queries.GetMyProfile;

namespace QuanLyHoaLan.API.Controllers;

[Authorize]
public class ProfileController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        return OkResult(await Mediator.Send(new GetMyProfileQuery()));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateMyProfileCommand command)
    {
        return OkResult(await Mediator.Send(command));
    }
}
