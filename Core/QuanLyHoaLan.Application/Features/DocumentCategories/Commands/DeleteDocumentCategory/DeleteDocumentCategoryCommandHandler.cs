using System.Linq.Expressions;
using MediatR;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Exceptions;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.DocumentCategories.Commands.DeleteDocumentCategory;

public class DeleteDocumentCategoryCommandHandler
    : IRequestHandler<DeleteDocumentCategoryCommand, bool>
{
    private readonly IBaseRepository<DocumentCategory> _categoryRepository;
    private readonly IBaseRepository<AppDocument> _documentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDocumentCategoryCommandHandler(
        IBaseRepository<DocumentCategory> categoryRepository,
        IBaseRepository<AppDocument> documentRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _documentRepository = documentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(
        DeleteDocumentCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.FindByIdAsync(command.Id);
        if (category == null)
        {
            throw new NotFoundException(nameof(DocumentCategory), command.Id);
        }

        Expression<Func<DocumentCategory, bool>>[] childFilter =
            [item => item.ParentId == command.Id];
        if (await _categoryRepository.AnyAsync(childFilter))
        {
            throw new InvalidOperationException("Không thể xóa danh mục đang có danh mục con.");
        }

        Expression<Func<AppDocument, bool>>[] documentFilter =
            [document => document.CategoryId == command.Id];
        if (await _documentRepository.AnyAsync(documentFilter))
        {
            throw new InvalidOperationException("Không thể xóa danh mục đang chứa tài liệu.");
        }

        await _categoryRepository.DeleteAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
