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
        Expression<Func<Orchid, bool>>[]? filters = null;
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            var searchLower = request.SearchTerm.ToLower();
            filters = new Expression<Func<Orchid, bool>>[] 
            { 
                x => x.Name.ToLower().Contains(searchLower) || x.EnglishName.ToLower().Contains(searchLower) 
            };
        }

        var skip = (request.PageNumber - 1) * request.PageSize;

        string orderBy = "DisplayOrder, CreatedAt desc";
        if (!string.IsNullOrEmpty(request.SortBy))
        {
            orderBy = request.SortBy;
            if (request.SortDescending)
            {
                orderBy += " desc";
            }
        }

        var includes = new Expression<Func<Orchid, object>>[] { x => x.Categories };

        var result = await _orchidRepository.FindResultAsync(filters, orderBy, skip, request.PageSize, includes);

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
            Slug = orchid.Slug,
            UploadedImageIds = orchid.UploadedImageIds,
            DisplayOrder = orchid.DisplayOrder
        }).ToList();

        return PaginatedList<OrchidDto>.Create(dtos, result.TotalCount, request.PageNumber, request.PageSize);
    }
}
