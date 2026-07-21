using System.Linq.Expressions;
using MediatR;
using QuanLyHoaLan.Application.Common.Models;
using QuanLyHoaLan.Application.DTOs.ArticleCategory;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.ArticleCategories.Queries.GetArticleCategories;

public class GetArticleCategoriesQueryHandler
    : IRequestHandler<GetArticleCategoriesQuery, PaginatedList<ArticleCategoryDto>>
{
    private readonly IBaseRepository<ArticleCategory> _categoryRepository;

    public GetArticleCategoriesQueryHandler(IBaseRepository<ArticleCategory> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<PaginatedList<ArticleCategoryDto>> Handle(
        GetArticleCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var filters = new List<Expression<Func<ArticleCategory, bool>>>();
        filters.Add(category => category.Type == request.Type);
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var search = request.SearchTerm.Trim().ToLowerInvariant();
            filters.Add(category => category.Name.ToLower().Contains(search));
        }

        if (request.ParentId.HasValue)
        {
            filters.Add(category => category.ParentId == request.ParentId);
        }

        var sortBy = request.SortBy?.Trim().ToLowerInvariant() switch
        {
            "slug" => "Slug",
            "createdat" => "CreatedAt",
            _ => "Name"
        };
        var orderBy = request.SortDescending ? $"{sortBy} desc" : sortBy;
        var skip = (request.PageNumber - 1) * request.PageSize;

        Expression<Func<ArticleCategory, object>>[] includes = [category => category.ParentCategory!];
        var result = await _categoryRepository.FindResultAsync(
            filters.ToArray(),
            orderBy,
            skip,
            request.PageSize,
            includes);

        var items = result.Items.Select(category =>
            ArticleCategoryTree.Map(category, category.ParentCategory?.Name ?? string.Empty)).ToList();

        return PaginatedList<ArticleCategoryDto>.Create(
            items,
            result.TotalCount,
            request.PageNumber,
            request.PageSize);
    }
}
