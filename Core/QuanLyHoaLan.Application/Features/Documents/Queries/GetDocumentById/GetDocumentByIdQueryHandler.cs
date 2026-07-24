using MediatR;
using QuanLyHoaLan.Application.DTOs.Document;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Exceptions;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Documents.Queries.GetDocumentById;

public class GetDocumentByIdQueryHandler : IRequestHandler<GetDocumentByIdQuery, AppDocumentDto>
{
    private readonly IBaseRepository<AppDocument> _documentRepository;

    public GetDocumentByIdQueryHandler(IBaseRepository<AppDocument> documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task<AppDocumentDto> Handle(
        GetDocumentByIdQuery request,
        CancellationToken cancellationToken)
    {
        var document = await _documentRepository.FindByIdAsync(
            request.Id,
            item => item.Category!);
        if (document == null)
        {
            throw new NotFoundException(nameof(AppDocument), request.Id);
        }

        return new AppDocumentDto
        {
            Id = document.Id,
            Title = document.Title,
            Description = document.Description,
            OriginalName = document.OriginalName,
            Extension = document.Extension,
            SizeBytes = document.SizeBytes,
            Url = document.Url,
            CategoryId = document.CategoryId,
            CategoryName = document.Category?.Name ?? string.Empty,
            CategorySlug = document.Category?.Slug ?? string.Empty,
            CreatedAt = document.CreatedAt
        };
    }
}
