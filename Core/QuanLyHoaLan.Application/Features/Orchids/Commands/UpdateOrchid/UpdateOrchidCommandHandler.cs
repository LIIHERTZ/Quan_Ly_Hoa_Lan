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

namespace QuanLyHoaLan.Application.Features.Orchids.Commands.UpdateOrchid;

public class UpdateOrchidCommandHandler : IRequestHandler<UpdateOrchidCommand, bool>
{
    private readonly IBaseRepository<Orchid> _orchidRepository;
    private readonly IBaseRepository<Category> _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOrchidCommandHandler(
        IBaseRepository<Orchid> orchidRepository, 
        IBaseRepository<Category> categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _orchidRepository = orchidRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateOrchidCommand command, CancellationToken cancellationToken)
    {
        var orchid = await _orchidRepository.FindByIdAsync(command.Id, x => x.Categories);

        if (orchid == null)
        {
            throw new Exception($"Không tìm thấy Orchid với Id {command.Id}.");
        }

        if (orchid.Slug != command.Slug)
        {
            Expression<Func<Orchid, bool>>[] filters = new Expression<Func<Orchid, bool>>[] { x => x.Slug == command.Slug };
            bool slugExists = await _orchidRepository.AnyAsync(filters);
            if (slugExists)
            {
                throw new Exception("Slug đã tồn tại.");
            }
        }

        orchid.Name = command.Name;
        orchid.EnglishName = command.EnglishName;
        orchid.ShortDescription = command.ShortDescription;
        orchid.DetailedDescription = command.DetailedDescription;
        orchid.HasFragrance = command.HasFragrance;
        orchid.IsPopular = command.IsPopular;
        orchid.Colors = NormalizeValues(command.Colors);
        orchid.Regions = NormalizeValues(command.Regions);
        orchid.BloomSeasons = NormalizeValues(command.BloomSeasons);
        orchid.Slug = command.Slug;
        orchid.UploadedImageIds = command.UploadedImageIds;
        orchid.DisplayOrder = command.DisplayOrder;

        orchid.Categories.Clear();
        if (command.CategoryIds != null && command.CategoryIds.Any())
        {
            Expression<Func<Category, bool>>[] catFilters = new Expression<Func<Category, bool>>[] { c => command.CategoryIds.Contains(c.Id) };
            var catsResult = await _categoryRepository.FindResultAsync(catFilters, "CreatedAt", 0, command.CategoryIds.Count, null);
            foreach(var c in catsResult.Items)
            {
                orchid.Categories.Add(c);
            }
        }

        await _orchidRepository.UpdateAsync(orchid, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
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
