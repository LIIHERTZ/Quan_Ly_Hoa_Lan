using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QuanLyHoaLan.Application.Common.Models;
using QuanLyHoaLan.Application.DTOs.Orchid;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Exceptions;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Orchids.Queries.GetOrchids;

public class GetOrchidsQueryHandler : IRequestHandler<GetOrchidsQuery, PaginatedList<OrchidDto>>
{
    private readonly IBaseRepository<Orchid> _orchidRepository;
    private readonly IBaseRepository<UploadedImage> _imageRepository;
    private readonly IBaseRepository<Category> _categoryRepository;

    public GetOrchidsQueryHandler(
        IBaseRepository<Orchid> orchidRepository,
        IBaseRepository<UploadedImage> imageRepository,
        IBaseRepository<Category> categoryRepository)
    {
        _orchidRepository = orchidRepository;
        _imageRepository = imageRepository;
        _categoryRepository = categoryRepository;
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

        var expandedCategoryIds = await ExpandCategoryIdsAsync(request.CategoryIds);
        if (expandedCategoryIds.Count > 0)
        {
            filters.Add(orchid => orchid.Categories.Any(
                category => expandedCategoryIds.Contains(category.Id)));
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

        var imageIds = result.Items
            .SelectMany(orchid => orchid.UploadedImageIds)
            .Distinct()
            .ToList();
        var images = imageIds.Count == 0
            ? new List<UploadedImage>()
            : await _imageRepository.FindAsync([image => imageIds.Contains(image.Id)], limit: int.MaxValue);
        var imagesById = images.ToDictionary(image => image.Id);

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
            UploadedImages = orchid.UploadedImageIds
                .Where(imagesById.ContainsKey)
                .Select(imageId => MapImage(imagesById[imageId]))
                .ToList(),
            DisplayOrder = orchid.DisplayOrder
        }).ToList();

        return PaginatedList<OrchidDto>.Create(dtos, result.TotalCount, request.PageNumber, request.PageSize);
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

    private async Task<List<Guid>> ExpandCategoryIdsAsync(IEnumerable<Guid>? requestedIds)
    {
        var selectedIds = requestedIds?.Distinct().ToList() ?? new List<Guid>();
        if (selectedIds.Count == 0)
        {
            return new List<Guid>();
        }

        var categories = await _categoryRepository.FindAsync(limit: int.MaxValue);
        var existingIds = categories.Select(category => category.Id).ToHashSet();
        var missingIds = selectedIds.Where(id => !existingIds.Contains(id)).ToList();
        if (missingIds.Count > 0)
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                [nameof(GetOrchidsQuery.CategoryIds)] =
                ["Một hoặc nhiều danh mục không tồn tại hoặc đã bị xóa."]
            });
        }

        var childrenByParentId = categories
            .Where(category => category.ParentId.HasValue)
            .GroupBy(category => category.ParentId!.Value)
            .ToDictionary(
                group => group.Key,
                group => group.Select(category => category.Id).ToList());

        var expandedIds = new HashSet<Guid>();
        var pendingIds = new Queue<Guid>(selectedIds);
        while (pendingIds.TryDequeue(out var categoryId))
        {
            if (!expandedIds.Add(categoryId)
                || !childrenByParentId.TryGetValue(categoryId, out var childIds))
            {
                continue;
            }

            foreach (var childId in childIds)
            {
                pendingIds.Enqueue(childId);
            }
        }

        return expandedIds.ToList();
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
