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
    private readonly IBaseRepository<UploadedImage> _imageRepository;

    public GetOrchidByIdQueryHandler(
        IBaseRepository<Orchid> orchidRepository,
        IBaseRepository<UploadedImage> imageRepository)
    {
        _orchidRepository = orchidRepository;
        _imageRepository = imageRepository;
    }

    public async Task<OrchidDto> Handle(GetOrchidByIdQuery request, CancellationToken cancellationToken)
    {
        var orchid = await _orchidRepository.FindByIdAsync(request.Id, x => x.Categories);
        if (orchid == null)
        {
            throw new Exception($"Không tìm thấy Orchid với Id {request.Id}.");
        }

        var images = orchid.UploadedImageIds.Count == 0
            ? new List<UploadedImage>()
            : await _imageRepository.FindAsync(
                [image => orchid.UploadedImageIds.Contains(image.Id)],
                limit: int.MaxValue);
        var imagesById = images.ToDictionary(image => image.Id);

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
            Colors = OrchidEnumValue.ParseStoredValues<FlowerColor>(orchid.Colors),
            Regions = OrchidEnumValue.ParseStoredValues<Region>(orchid.Regions),
            BloomSeasons = OrchidEnumValue.ParseStoredValues<BloomSeason>(orchid.BloomSeasons),
            Slug = orchid.Slug,
            UploadedImageIds = orchid.UploadedImageIds,
            UploadedImages = orchid.UploadedImageIds
                .Where(imagesById.ContainsKey)
                .Select(imageId => MapImage(imagesById[imageId]))
                .ToList(),
            DisplayOrder = orchid.DisplayOrder
        };
    }

    private static OrchidImageDto MapImage(UploadedImage image)
    {
        return new OrchidImageDto
        {
            Id = image.Id,
            Url = image.Url,
            PublicId = image.PublicId,
            FileName = image.FileName
        };
    }
}
