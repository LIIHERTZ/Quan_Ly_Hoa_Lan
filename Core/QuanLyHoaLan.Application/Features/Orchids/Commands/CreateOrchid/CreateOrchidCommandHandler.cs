using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QuanLyHoaLan.Application.DTOs.Orchid;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Orchids.Commands.CreateOrchid;

public class CreateOrchidCommandHandler : IRequestHandler<CreateOrchidCommand, Guid>
{
    private readonly IBaseRepository<Orchid> _orchidRepository;
    private readonly IBaseRepository<Category> _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrchidCommandHandler(
        IBaseRepository<Orchid> orchidRepository, 
        IBaseRepository<Category> categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _orchidRepository = orchidRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateOrchidCommand command, CancellationToken cancellationToken)
    {
        Expression<Func<Orchid, bool>>[] filters = new Expression<Func<Orchid, bool>>[] { x => x.Slug == command.Slug };
        bool slugExists = await _orchidRepository.AnyAsync(filters);
        if (slugExists)
        {
            throw new Exception("Slug đã tồn tại.");
        }

        var orchid = new Orchid
        {
            Name = command.Name,
            EnglishName = command.EnglishName,
            ShortDescription = command.ShortDescription,
            DetailedDescription = command.DetailedDescription,
            HasFragrance = command.HasFragrance,
            IsPopular = command.IsPopular,
            Colors = NormalizeValues(command.Colors),
            Regions = NormalizeValues(command.Regions),
            BloomSeasons = NormalizeValues(command.BloomSeasons),
            Slug = command.Slug,
            UploadedImageIds = command.UploadedImageIds,
            DisplayOrder = command.DisplayOrder
        };

        if (command.CategoryIds != null && command.CategoryIds.Any())
        {
            Expression<Func<Category, bool>>[] catFilters = new Expression<Func<Category, bool>>[] { c => command.CategoryIds.Contains(c.Id) };
            var catsResult = await _categoryRepository.FindResultAsync(catFilters, "CreatedAt", 0, command.CategoryIds.Count, null);
            orchid.Categories = catsResult.Items.ToList();
        }

        await _orchidRepository.InsertAsync(orchid, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return orchid.Id;
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
