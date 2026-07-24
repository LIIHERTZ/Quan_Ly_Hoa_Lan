using System.Linq.Expressions;
using MediatR;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Exceptions;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Documents.Commands.UpdateDocument;

public class UpdateDocumentCommandHandler : IRequestHandler<UpdateDocumentCommand, bool>
{
    private readonly IBaseRepository<AppDocument> _documentRepository;
    private readonly IBaseRepository<DocumentCategory> _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDocumentCommandHandler(
        IBaseRepository<AppDocument> documentRepository,
        IBaseRepository<DocumentCategory> categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _documentRepository = documentRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(
        UpdateDocumentCommand command,
        CancellationToken cancellationToken)
    {
        var document = await _documentRepository.FindByIdAsync(command.Id);
        if (document == null)
        {
            throw new NotFoundException(nameof(AppDocument), command.Id);
        }

        var category = await _categoryRepository.FindByIdAsync(command.CategoryId);
        if (category == null)
        {
            throw new InvalidOperationException("Danh mục tài liệu không tồn tại hoặc đã bị xóa.");
        }

        Expression<Func<DocumentCategory, bool>>[] childFilters =
            [item => item.ParentId == command.CategoryId];
        if (await _categoryRepository.AnyAsync(childFilters))
        {
            throw new InvalidOperationException(
                "Tài liệu chỉ được gắn với danh mục lá, không được gắn trực tiếp vào danh mục cha.");
        }

        document.Title = command.Title.Trim();
        document.Description = command.Description?.Trim();
        document.CategoryId = category.Id;

        await _documentRepository.UpdateAsync(document, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
