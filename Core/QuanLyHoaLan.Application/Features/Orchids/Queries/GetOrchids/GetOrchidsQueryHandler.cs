using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QuanLyHoaLan.Application.Common.Models;
using QuanLyHoaLan.Application.DTOs.Orchid;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Orchids.Queries.GetOrchids;

public class GetOrchidsQueryHandler : IRequestHandler<GetOrchidsQuery, PaginatedList<OrchidDto>>
{
    private readonly IBaseRepository<Orchid> _orchidRepository;

    public GetOrchidsQueryHandler(IBaseRepository<Orchid> orchidRepository)
    {
        _orchidRepository = orchidRepository;
    }

    public async Task<PaginatedList<OrchidDto>> Handle(GetOrchidsQuery request, CancellationToken cancellationToken)
    {
        var filters = new List<Expression<Func<Orchid, bool>>>();
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchLower = request.SearchTerm.Trim().ToLowerInvariant();
            filters.Add(orchid => orchid.Name.ToLower().Contains(searchLower)
                || orchid.EnglishName.ToLower().Contains(searchLower));
        }

        if (request.IsPopular.HasValue)
        {
            filters.Add(orchid => orchid.IsPopular == request.IsPopular.Value);
        }

        if (request.HasFragrance.HasValue)
        {
            filters.Add(orchid => orchid.HasFragrance == request.HasFragrance.Value);
        }

        var colors = NormalizeValues(request.Colors);
        if (colors.Count > 0)
        {
            filters.Add(orchid => orchid.Colors.Any(color => colors.Contains(color)));
        }

        var regions = NormalizeValues(request.Regions);
        if (regions.Count > 0)
        {
            filters.Add(orchid => orchid.Regions.Any(region => regions.Contains(region)));
        }

        var bloomSeasons = NormalizeValues(request.BloomSeasons);
        if (bloomSeasons.Count > 0)
        {
            filters.Add(orchid => orchid.BloomSeasons.Any(season => bloomSeasons.Contains(season)));
        }

        var skip = (request.PageNumber - 1) * request.PageSize;

        var sortBy = request.SortBy?.Trim().ToLowerInvariant() switch
        {
            "name" => "Name",
            "englishname" => "EnglishName",
            "createdat" => "CreatedAt",
            "ispopular" => "IsPopular",
            _ => "DisplayOrder"
        };
        var orderBy = request.SortDescending ? $"{sortBy} desc" : sortBy;

        var includes = new Expression<Func<Orchid, object>>[] { x => x.Categories };

        var result = await _orchidRepository.FindResultAsync(
            filters.Count == 0 ? null : filters.ToArray(),
            orderBy,
            skip,
            request.PageSize,
            includes);

        var dtos = result.Items.Select(orchid => new OrchidDto
        {
            Id = orchid.Id,
            Name = orchid.Name,
            EnglishName = orchid.EnglishName,
            Categories = orchid.Categories.Select(c => new QuanLyHoaLan.Application.DTOs.Orchid.SimpleCategoryDto 
            { 
                Id = c.Id, 
                Name = c.Name 
            }).ToList(),
            ShortDescription = orchid.ShortDescription,
            DetailedDescription = orchid.DetailedDescription,
            HasFragrance = orchid.HasFragrance,
            IsPopular = orchid.IsPopular,
            Colors = OrchidEnumValue.ParseStoredValues<FlowerColor>(orchid.Colors),
            Regions = OrchidEnumValue.ParseStoredValues<Region>(orchid.Regions),
            BloomSeasons = OrchidEnumValue.ParseStoredValues<BloomSeason>(orchid.BloomSeasons),
            Slug = orchid.Slug,
            UploadedImageIds = orchid.UploadedImageIds,
            DisplayOrder = orchid.DisplayOrder
        }).ToList();

        return PaginatedList<OrchidDto>.Create(dtos, result.TotalCount, request.PageNumber, request.PageSize);
    }

    private static List<string> NormalizeValues<TEnum>(IEnumerable<TEnum>? values)
        where TEnum : struct, Enum
    {
        return values?
            .Distinct()
            .Select(value => value.ToStorageValue())
            .ToList() ?? new List<string>();
    }
}
