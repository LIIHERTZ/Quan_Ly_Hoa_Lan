using MediatR;
using QuanLyHoaLan.Application.Common.Models;
using QuanLyHoaLan.Application.DTOs.Orchid;

namespace QuanLyHoaLan.Application.Features.Orchids.Queries.GetOrchids;

public class GetOrchidsQuery : PagedRequest, IRequest<PaginatedList<OrchidDto>>
{
    public List<Guid> CategoryIds { get; set; } = new();
    public bool? IsPopular { get; set; }
    public bool? HasFragrance { get; set; }
    public List<FlowerColor> Colors { get; set; } = new();
    public List<Region> Regions { get; set; } = new();
    public List<BloomSeason> BloomSeasons { get; set; } = new();
}
