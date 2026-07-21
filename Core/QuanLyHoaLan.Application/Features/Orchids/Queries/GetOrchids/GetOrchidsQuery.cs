using MediatR;
using QuanLyHoaLan.Application.Common.Models;
using QuanLyHoaLan.Application.DTOs.Orchid;

namespace QuanLyHoaLan.Application.Features.Orchids.Queries.GetOrchids;

public class GetOrchidsQuery : PagedRequest, IRequest<PaginatedList<OrchidDto>>
{
    public bool? IsPopular { get; set; }
    public bool? HasFragrance { get; set; }
    public List<string> Colors { get; set; } = new();
    public List<string> Regions { get; set; } = new();
    public List<string> BloomSeasons { get; set; } = new();
}
