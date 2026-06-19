using MediatR;
using QuanLyHoaLan.Application.DTOs.Document;
using QuanLyHoaLan.Application.Common.Models;

namespace QuanLyHoaLan.Application.Features.Documents.Queries.GetDocuments;

public class GetDocumentsQuery : IRequest<PaginatedList<AppDocumentDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
}
