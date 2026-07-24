using System.Linq.Expressions;
using MediatR;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Exceptions;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.DocumentCategories.Commands.UpdateDocumentCategory;

public class UpdateDocumentCategoryCommandHandler
    : IRequestHandler<UpdateDocumentCategoryCommand, bool>
{
    private readonly IBaseRepository<DocumentCategory> _categoryRepository;
    private readonly IBaseRepository<AppDocument> _documentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDocumentCategoryCommandHandler(
        IBaseRepository<DocumentCategory> categoryRepository,
        IBaseRepository<AppDocument> documentRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _documentRepository = documentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(
        UpdateDocumentCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.FindByIdAsync(command.Id);
        if (category == null)
        {
            throw new NotFoundException(nameof(DocumentCategory), command.Id);
        }

        var slug = command.Slug.Trim().ToLowerInvariant();
        Expression<Func<DocumentCategory, bool>>[] slugFilter =
            [item => item.Id != command.Id && item.Slug == slug];
        if (await _categoryRepository.AnyAsync(slugFilter))
        {
            throw new InvalidOperationException("Slug danh mục tài liệu đã tồn tại.");
        }

        var categories = await _categoryRepository.FindAsync(limit: int.MaxValue);
        DocumentCategoryTree.EnsureValidParent(command.Id, command.ParentId, categories);
        if (category.ParentId != command.ParentId)
        {
            await EnsureParentIsNotUsedByDocument(command.ParentId);
        }

        category.Name = command.Name.Trim();
        category.Description = command.Description?.Trim() ?? string.Empty;
        category.Slug = slug;
        category.ParentId = command.ParentId;

        await _categoryRepository.UpdateAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task EnsureParentIsNotUsedByDocument(Guid? parentId)
    {
        if (!parentId.HasValue)
        {
            return;
        }

        Expression<Func<AppDocument, bool>>[] filters =
            [document => document.CategoryId == parentId.Value];
        if (await _documentRepository.AnyAsync(filters))
        {
            throw new InvalidOperationException(
                "Không thể chuyển vào danh mục cha đang chứa tài liệu trực tiếp.");
        }
    }
}
