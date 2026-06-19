using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QuanLyHoaLan.Application.Common.Models;
using QuanLyHoaLan.Application.DTOs.Category;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Categories.Queries.GetCategories;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, PaginatedList<CategoryDto>>
{
    private readonly IBaseRepository<Category> _categoryRepository;

    public GetCategoriesQueryHandler(IBaseRepository<Category> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<PaginatedList<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Category, bool>>[]? filters = null;
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            filters = new Expression<Func<Category, bool>>[] 
            { 
                x => x.Name.Contains(request.SearchTerm) 
            };
        }

        var skip = (request.PageNumber - 1) * request.PageSize;

        string orderBy = "CreatedAt";
        if (!string.IsNullOrEmpty(request.SortBy))
        {
            orderBy = request.SortBy;
        }
        if (request.SortDescending)
        {
            orderBy += " desc";
        }

        var includes = new Expression<Func<Category, object>>[] { x => x.ParentCategory };

        var result = await _categoryRepository.FindResultAsync(filters, orderBy, skip, request.PageSize, includes);

        var dtos = result.Items.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            Slug = c.Slug,
            ParentId = c.ParentId,
            ParentName = c.ParentCategory?.Name ?? string.Empty
        }).ToList();

        return PaginatedList<CategoryDto>.Create(dtos, result.TotalCount, request.PageNumber, request.PageSize);
    }
}
