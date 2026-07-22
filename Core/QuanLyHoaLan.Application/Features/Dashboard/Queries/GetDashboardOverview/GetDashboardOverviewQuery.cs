using MediatR;
using QuanLyHoaLan.Application.DTOs.Dashboard;

namespace QuanLyHoaLan.Application.Features.Dashboard.Queries.GetDashboardOverview;

public record GetDashboardOverviewQuery : IRequest<DashboardOverviewDto>;
