using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using QuanLyHoaLan.Application.Common.Models;

namespace QuanLyHoaLan.API.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public abstract class BaseController : ControllerBase
{
    private ISender? _mediator;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    protected IActionResult OkResult<T>(T data, string message = "Thành công")
    {
        return Ok(ApiResponse<T>.Ok(data, message));
    }

    protected IActionResult OkResult(string message = "Thành công")
    {
        return Ok(ApiResponse<object>.Ok(null, message));
    }
}
