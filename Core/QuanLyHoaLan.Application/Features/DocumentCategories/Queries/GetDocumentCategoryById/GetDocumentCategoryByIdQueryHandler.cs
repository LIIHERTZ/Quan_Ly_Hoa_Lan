using MediatR;
using QuanLyHoaLan.Application.DTOs.DocumentCategory;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Exceptions;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.DocumentCategories.Queries.GetDocumentCategoryById;

public class GetDocumentCategoryByIdQueryHandler
    : IRequestHandler<GetDocumentCategoryByIdQuery, DocumentCategoryDto>
{
    private readonly IBaseRepository<DocumentCategory> _categoryRepository;
    private readonly IBaseRepository<AppDocument> _documentRepository;

    public GetDocumentCategoryByIdQueryHandler(
        IBaseRepository<DocumentCategory> categoryRepository,
        IBaseRepository<AppDocument> documentRepository)
    {
        _categoryRepository = categoryRepository;
        _documentRepository = documentRepository;
    }

    public async Task<DocumentCategoryDto> Handle(
        GetDocumentCategoryByIdQuery request,
        CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.FindByIdAsync(
            request.Id,
            item => item.ParentCategory!);
        if (category == null)
        {
            throw new NotFoundException(nameof(DocumentCategory), request.Id);
        }

        var categories = await _categoryRepository.FindAsync(limit: int.MaxValue);
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

        var dto = DocumentCategoryTree.Map(
            category,
            category.ParentCategory?.Name ?? string.Empty,
            DocumentCategoryTree.CountDocumentsIncludingDescendants(
                category.Id,
                categories,
                documentCounts));
        dto.SubCategories = DocumentCategoryTree.Build(
            categories,
            documentCounts,
            category.Id);
        return dto;
    }
}
