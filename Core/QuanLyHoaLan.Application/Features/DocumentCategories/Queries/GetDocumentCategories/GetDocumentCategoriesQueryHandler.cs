using System.Linq.Expressions;
using MediatR;
using QuanLyHoaLan.Application.Common.Models;
using QuanLyHoaLan.Application.DTOs.DocumentCategory;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.DocumentCategories.Queries.GetDocumentCategories;

public class GetDocumentCategoriesQueryHandler
    : IRequestHandler<GetDocumentCategoriesQuery, PaginatedList<DocumentCategoryDto>>
{
    private readonly IBaseRepository<DocumentCategory> _categoryRepository;
    private readonly IBaseRepository<AppDocument> _documentRepository;

    public GetDocumentCategoriesQueryHandler(
        IBaseRepository<DocumentCategory> categoryRepository,
        IBaseRepository<AppDocument> documentRepository)
    {
        _categoryRepository = categoryRepository;
        _documentRepository = documentRepository;
    }

    public async Task<PaginatedList<DocumentCategoryDto>> Handle(
        GetDocumentCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var filters = new List<Expression<Func<DocumentCategory, bool>>>();
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var search = request.SearchTerm.Trim().ToLower();
            filters.Add(category =>
                category.Name.ToLower().Contains(search) ||
                category.Slug.ToLower().Contains(search) ||
                category.Description.ToLower().Contains(search));
        }

        if (request.ParentId.HasValue)
        {
            filters.Add(category => category.ParentId == request.ParentId.Value);
        }

        var result = await _categoryRepository.FindResultAsync(
            filters.Count == 0 ? null : filters.ToArray(),
            "Name",
            (request.PageNumber - 1) * request.PageSize,
            request.PageSize,
            [category => category.ParentCategory!]);

        var documentCategoryIds = await _documentRepository.FindProjectedResultAsync(
            filters: null,
            orderBy: null,
            skip: 0,
            limit: int.MaxValue,
            selector: document => document.CategoryId);
        var documentCounts = documentCategoryIds.Items
            .Where(categoryId => categoryId.HasValue)
            .Select(categoryId => categoryId!.Value)
            .GroupBy(categoryId => categoryId)
            .ToDictionary(group => group.Key, group => group.Count());
        var allCategories = await _categoryRepository.FindAsync(limit: int.MaxValue);

        var items = result.Items.Select(category =>
            DocumentCategoryTree.Map(
                category,
                category.ParentCategory?.Name ?? string.Empty,
                DocumentCategoryTree.CountDocumentsIncludingDescendants(
                    category.Id,
                    allCategories,
                    documentCounts))).ToList();

        return PaginatedList<DocumentCategoryDto>.Create(
            items,
            result.TotalCount,
            request.PageNumber,
            request.PageSize);
    }
}
