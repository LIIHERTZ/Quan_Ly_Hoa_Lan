using MediatR;
using QuanLyHoaLan.Application.Interfaces.Services;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Exceptions;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Documents.Commands.DeleteDocument;

public class DeleteDocumentCommandHandler : IRequestHandler<DeleteDocumentCommand, bool>
{
    private readonly IDocumentService _documentService;
    private readonly IBaseRepository<AppDocument> _documentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDocumentCommandHandler(
        IDocumentService documentService,
        IBaseRepository<AppDocument> documentRepository,
        IUnitOfWork unitOfWork)
    {
        _documentService = documentService;
        _documentRepository = documentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await _documentRepository.FindByIdAsync(request.Id);

        if (document == null)
        {
            throw new NotFoundException(nameof(AppDocument), request.Id);
        }

        if (!string.IsNullOrEmpty(document.PublicId))
        {
            await _documentService.DeleteDocumentAsync(document.PublicId);
        }

        await _documentRepository.DeleteAsync(document, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
