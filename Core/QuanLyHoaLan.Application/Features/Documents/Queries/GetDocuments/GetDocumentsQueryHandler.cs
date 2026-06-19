using System.Linq.Expressions;
using MediatR;
using QuanLyHoaLan.Application.Common.Models;
using QuanLyHoaLan.Application.DTOs.Document;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Documents.Queries.GetDocuments;

public class GetDocumentsQueryHandler : IRequestHandler<GetDocumentsQuery, PaginatedList<AppDocumentDto>>
{
    private readonly IBaseRepository<AppDocument> _documentRepository;

    public GetDocumentsQueryHandler(IBaseRepository<AppDocument> documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task<PaginatedList<AppDocumentDto>> Handle(GetDocumentsQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<AppDocument, bool>>[]? filters = null;
        var searchLower = request.SearchTerm?.ToLower();

        if (!string.IsNullOrEmpty(searchLower))
        {
            filters = new Expression<Func<AppDocument, bool>>[]
            {
                x => x.Title.ToLower().Contains(searchLower) || (x.Description != null && x.Description.ToLower().Contains(searchLower))
            };
        }

        var skip = (request.PageNumber - 1) * request.PageSize;

        var result = await _documentRepository.FindResultAsync(
            filters: filters,
            orderBy: "CreatedAt descending",
            skip: skip,
            limit: request.PageSize
        );

        var dtos = result.Items.Select(d => new AppDocumentDto
        {
            Id = d.Id,
            Title = d.Title,
            Description = d.Description,
            OriginalName = d.OriginalName,
            Extension = d.Extension,
            SizeBytes = d.SizeBytes,
            Url = d.Url,
            CreatedAt = d.CreatedAt
        }).ToList();

        return PaginatedList<AppDocumentDto>.Create(dtos, result.TotalCount, request.PageNumber, request.PageSize);
    }
}
