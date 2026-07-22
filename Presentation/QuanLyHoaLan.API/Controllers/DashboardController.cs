using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyHoaLan.Application.Features.Dashboard.Queries.GetDashboardOverview;
using QuanLyHoaLan.Domain.Constants;

namespace QuanLyHoaLan.API.Controllers;

[Authorize(Roles = RoleConstants.Admin)]
public class DashboardController : BaseController
{
    [HttpGet("overview")]
    public async Task<IActionResult> GetOverview()
    {
        return OkResult(await Mediator.Send(new GetDashboardOverviewQuery()));
    }
}
