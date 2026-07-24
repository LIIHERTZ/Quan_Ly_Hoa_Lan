using System.Linq.Expressions;
using MediatR;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.DocumentCategories.Commands.CreateDocumentCategory;

public class CreateDocumentCategoryCommandHandler
    : IRequestHandler<CreateDocumentCategoryCommand, Guid>
{
    private readonly IBaseRepository<DocumentCategory> _categoryRepository;
    private readonly IBaseRepository<AppDocument> _documentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDocumentCategoryCommandHandler(
        IBaseRepository<DocumentCategory> categoryRepository,
        IBaseRepository<AppDocument> documentRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _documentRepository = documentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        CreateDocumentCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var slug = command.Slug.Trim().ToLowerInvariant();
        Expression<Func<DocumentCategory, bool>>[] slugFilter =
            [category => category.Slug == slug];
        if (await _categoryRepository.AnyAsync(slugFilter))
        {
            throw new InvalidOperationException("Slug danh mục tài liệu đã tồn tại.");
        }

        var categories = await _categoryRepository.FindAsync(limit: int.MaxValue);
        DocumentCategoryTree.EnsureValidParent(Guid.NewGuid(), command.ParentId, categories);
        await EnsureParentIsNotUsedByDocument(command.ParentId);

        var category = new DocumentCategory
        {
            Name = command.Name.Trim(),
            Description = command.Description?.Trim() ?? string.Empty,
            Slug = slug,
            ParentId = command.ParentId
        };

        await _categoryRepository.InsertAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return category.Id;
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
                "Không thể thêm danh mục con vì danh mục cha đang chứa tài liệu trực tiếp.");
        }
    }
}
