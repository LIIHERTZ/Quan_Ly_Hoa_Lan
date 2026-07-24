using MediatR;
using QuanLyHoaLan.Application.DTOs.Document;

namespace QuanLyHoaLan.Application.Features.Documents.Queries.GetDocumentById;

public record GetDocumentByIdQuery(Guid Id) : IRequest<AppDocumentDto>;
