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
    private readonly IBaseRepository<DocumentCategory> _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UploadDocumentCommandHandler(
        IDocumentService documentService,
        IBaseRepository<AppDocument> documentRepository,
        IBaseRepository<DocumentCategory> categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _documentService = documentService;
        _documentRepository = documentRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AppDocumentDto> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.FindByIdAsync(request.CategoryId);
        if (category == null)
        {
            throw new InvalidOperationException("Danh mục tài liệu không tồn tại hoặc đã bị xóa.");
        }

        System.Linq.Expressions.Expression<Func<DocumentCategory, bool>>[] childFilters =
            [item => item.ParentId == request.CategoryId];
        if (await _categoryRepository.AnyAsync(childFilters))
        {
            throw new InvalidOperationException(
                "Tài liệu chỉ được gắn với danh mục lá, không được gắn trực tiếp vào danh mục cha.");
        }

        var uploadResult = await _documentService.UploadDocumentAsync(request.FileStream, request.FileName);

        if (uploadResult == null)
        {
            throw new InvalidOperationException("Failed to upload document to cloud storage.");
        }

        var document = new AppDocument
        {
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            OriginalName = request.FileName,
            Extension = uploadResult.Format,
            SizeBytes = uploadResult.Bytes,
            Url = uploadResult.Url,
            PublicId = uploadResult.PublicId,
            CategoryId = category.Id,
            Category = category
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
            CategoryId = category.Id,
            CategoryName = category.Name,
            CategorySlug = category.Slug,
            CreatedAt = document.CreatedAt
        };
    }
}
