using MediatR;
using QuanLyHoaLan.Application.DTOs.DocumentCategory;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.DocumentCategories.Queries.GetDocumentCategoryTree;

public class GetDocumentCategoryTreeQueryHandler
    : IRequestHandler<GetDocumentCategoryTreeQuery, List<DocumentCategoryDto>>
{
    private readonly IBaseRepository<DocumentCategory> _categoryRepository;
    private readonly IBaseRepository<AppDocument> _documentRepository;

    public GetDocumentCategoryTreeQueryHandler(
        IBaseRepository<DocumentCategory> categoryRepository,
        IBaseRepository<AppDocument> documentRepository)
    {
        _categoryRepository = categoryRepository;
        _documentRepository = documentRepository;
    }

    public async Task<List<DocumentCategoryDto>> Handle(
        GetDocumentCategoryTreeQuery request,
        CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.FindAsync(
            orderBy: "Name",
            limit: int.MaxValue);
        var documentCategoryIds = await _documentRepository.FindProjectedResultAsync(
            filters: null,
            orderBy: null,
            skip: 0,
            limit: int.MaxValue,
            selector: document => document.CategoryId);
        var documentCounts = documentCategoryIds.Items
            .GroupBy(categoryId => categoryId)
            .ToDictionary(group => group.Key, group => group.Count());

        return DocumentCategoryTree.Build(categories, documentCounts);
    }
}
