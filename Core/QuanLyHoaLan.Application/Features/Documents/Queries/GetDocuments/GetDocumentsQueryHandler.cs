using System.Linq.Expressions;
using MediatR;
using QuanLyHoaLan.Application.Common.Models;
using QuanLyHoaLan.Application.DTOs.Document;
using QuanLyHoaLan.Application.Features.DocumentCategories;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Documents.Queries.GetDocuments;

public class GetDocumentsQueryHandler
    : IRequestHandler<GetDocumentsQuery, PaginatedList<AppDocumentDto>>
{
    private readonly IBaseRepository<AppDocument> _documentRepository;
    private readonly IBaseRepository<DocumentCategory> _categoryRepository;

    public GetDocumentsQueryHandler(
        IBaseRepository<AppDocument> documentRepository,
        IBaseRepository<DocumentCategory> categoryRepository)
    {
        _documentRepository = documentRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<PaginatedList<AppDocumentDto>> Handle(
        GetDocumentsQuery request,
        CancellationToken cancellationToken)
    {
        var filters = new List<Expression<Func<AppDocument, bool>>>();
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var search = request.SearchTerm.Trim().ToLower();
            filters.Add(document =>
                document.Title.ToLower().Contains(search) ||
                (document.Description != null &&
                 document.Description.ToLower().Contains(search)));
        }

        if (request.CategoryId.HasValue)
        {
            var categories = await _categoryRepository.FindAsync(limit: int.MaxValue);
            var categoryIds = DocumentCategoryTree.ExpandWithDescendants(
                request.CategoryId.Value,
                categories);
            filters.Add(document => categoryIds.Contains(document.CategoryId));
        }

        var result = await _documentRepository.FindResultAsync(
            filters.Count == 0 ? null : filters.ToArray(),
            "CreatedAt descending",
            (request.PageNumber - 1) * request.PageSize,
            request.PageSize,
            [document => document.Category]);

        var dtos = result.Items.Select(document => new AppDocumentDto
        {
            Id = document.Id,
            Title = document.Title,
            Description = document.Description,
            OriginalName = document.OriginalName,
            Extension = document.Extension,
            SizeBytes = document.SizeBytes,
            Url = document.Url,
            CategoryId = document.CategoryId,
            CategoryName = document.Category.Name,
            CategorySlug = document.Category.Slug,
            CreatedAt = document.CreatedAt
        }).ToList();

        return PaginatedList<AppDocumentDto>.Create(
            dtos,
            result.TotalCount,
            request.PageNumber,
            request.PageSize);
    }
}
