using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QuanLyHoaLan.Application.DTOs.Category;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
{
    private readonly IBaseRepository<Category> _categoryRepository;

    public GetCategoryByIdQueryHandler(IBaseRepository<Category> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.FindByIdAsync(request.Id, x => x.ParentCategory);
        if (category == null)
        {
            throw new Exception($"Không tìm thấy Category với Id {request.Id}.");
        }

        var allCategoriesResult = await _categoryRepository.FindResultAsync(null, "CreatedAt", 0, int.MaxValue, null);
        var allCategories = allCategoriesResult.Items.ToList();

        var categoryDto = MapToDto(category, category.ParentCategory?.Name ?? string.Empty);

        BuildSubCategoryTree(categoryDto, allCategories);

        return categoryDto;
    }

    private void BuildSubCategoryTree(CategoryDto parentDto, List<Category> allCategories)
    {
        var children = allCategories.Where(c => c.ParentId == parentDto.Id).ToList();
        
        foreach (var child in children)
        {
            var childDto = MapToDto(child, parentDto.Name);
            parentDto.SubCategories.Add(childDto);
            
            BuildSubCategoryTree(childDto, allCategories);
        }
    }

    private CategoryDto MapToDto(Category c, string parentName)
    {
        return new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            Slug = c.Slug,
            ParentId = c.ParentId,
            ParentName = parentName
        };
    }
}
