using MediatR;
using QuanLyHoaLan.Application.DTOs.Document;
using QuanLyHoaLan.Application.Interfaces.Services;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Documents.Commands.UploadDocument;

public class UploadDocumentCommandHandler : IRequestHandler<UploadDocumentCommand, AppDocumentDto>
{
    private readonly IDocumentService _documentService;
    private readonly IBaseRepository<AppDocument> _documentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UploadDocumentCommandHandler(
        IDocumentService documentService,
        IBaseRepository<AppDocument> documentRepository,
        IUnitOfWork unitOfWork)
    {
        _documentService = documentService;
        _documentRepository = documentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AppDocumentDto> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
    {
        var uploadResult = await _documentService.UploadDocumentAsync(request.FileStream, request.FileName);

        if (uploadResult == null)
        {
            throw new InvalidOperationException("Failed to upload document to cloud storage.");
        }

        var document = new AppDocument
        {
            Title = request.Title,
            Description = request.Description,
            OriginalName = request.FileName,
            Extension = uploadResult.Format,
            SizeBytes = uploadResult.Bytes,
            Url = uploadResult.Url,
            PublicId = uploadResult.PublicId
        };

        await _documentRepository.InsertAsync(document, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AppDocumentDto
        {
            Id = document.Id,
            Title = document.Title,
            Description = document.Description,
            OriginalName = document.OriginalName,
            Extension = document.Extension,
            SizeBytes = document.SizeBytes,
            Url = document.Url,
            CreatedAt = document.CreatedAt
        };
    }
}
