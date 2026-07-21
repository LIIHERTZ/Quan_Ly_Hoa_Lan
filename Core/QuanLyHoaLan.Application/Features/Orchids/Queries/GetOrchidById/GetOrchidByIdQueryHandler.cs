using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QuanLyHoaLan.Application.DTOs.Orchid;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Orchids.Queries.GetOrchidById;

public class GetOrchidByIdQueryHandler : IRequestHandler<GetOrchidByIdQuery, OrchidDto>
{
    private readonly IBaseRepository<Orchid> _orchidRepository;

    public GetOrchidByIdQueryHandler(IBaseRepository<Orchid> orchidRepository)
    {
        _orchidRepository = orchidRepository;
    }

    public async Task<OrchidDto> Handle(GetOrchidByIdQuery request, CancellationToken cancellationToken)
    {
        var orchid = await _orchidRepository.FindByIdAsync(request.Id, x => x.Categories);
        if (orchid == null)
        {
            throw new Exception($"Không tìm thấy Orchid với Id {request.Id}.");
        }

        return new OrchidDto
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
            Colors = orchid.Colors,
            Regions = orchid.Regions,
            BloomSeasons = orchid.BloomSeasons,
            Slug = orchid.Slug,
            UploadedImageIds = orchid.UploadedImageIds,
            DisplayOrder = orchid.DisplayOrder
        };
    }
}
