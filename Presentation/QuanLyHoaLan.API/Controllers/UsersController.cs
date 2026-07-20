using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyHoaLan.Application.Features.Users.Commands.CreateUser;
using QuanLyHoaLan.Application.Features.Users.Commands.DeleteUser;
using QuanLyHoaLan.Application.Features.Users.Commands.ResetUserPassword;
using QuanLyHoaLan.Application.Features.Users.Commands.UpdateUser;
using QuanLyHoaLan.Application.Features.Users.Queries.GetUserById;
using QuanLyHoaLan.Application.Features.Users.Queries.GetUserRoles;
using QuanLyHoaLan.Application.Features.Users.Queries.GetUsers;
using QuanLyHoaLan.Domain.Constants;

namespace QuanLyHoaLan.API.Controllers;

[ApiVersion("1.0")]
[Authorize(Roles = RoleConstants.Admin)]
public class UsersController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] GetUsersQuery query)
    {
        return OkResult(await Mediator.Send(query));
    }

    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles()
    {
        return OkResult(await Mediator.Send(new GetUserRolesQuery()));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        return OkResult(await Mediator.Send(new GetUserByIdQuery(id)));
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
    {
        var user = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetUserById), new { id = user.Id, version = "1" }, user);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserCommand command)
    {
        if (command.Id != Guid.Empty && command.Id != id)
        {
            return BadRequest("ID trong URL và nội dung request không khớp.");
        }

        command.Id = id;
        return OkResult(await Mediator.Send(command));
    }

    [HttpPut("{id:guid}/password")]
    public async Task<IActionResult> ResetPassword(Guid id, [FromBody] ResetUserPasswordCommand command)
    {
        if (command.Id != Guid.Empty && command.Id != id)
        {
            return BadRequest("ID trong URL và nội dung request không khớp.");
        }

        command.Id = id;
        await Mediator.Send(command);
        return OkResult("Đặt lại mật khẩu thành công.");
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        await Mediator.Send(new DeleteUserCommand(id));
        return NoContent();
    }
}
