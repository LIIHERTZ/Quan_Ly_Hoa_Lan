using System.IO;
using MediatR;
using QuanLyHoaLan.Application.DTOs.Document;

namespace QuanLyHoaLan.Application.Features.Documents.Commands.UploadDocument;

public class UploadDocumentCommand : IRequest<AppDocumentDto>
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Stream FileStream { get; set; } = Stream.Null;
    public string FileName { get; set; } = string.Empty;
}
