using System.Linq.Expressions;
using MediatR;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Exceptions;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.ArticleCategories.Commands.UpdateArticleCategory;

public class UpdateArticleCategoryCommandHandler : IRequestHandler<UpdateArticleCategoryCommand, bool>
{
    private readonly IBaseRepository<ArticleCategory> _categoryRepository;
    private readonly IBaseRepository<Article> _articleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateArticleCategoryCommandHandler(
        IBaseRepository<ArticleCategory> categoryRepository,
        IBaseRepository<Article> articleRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _articleRepository = articleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateArticleCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.FindByIdAsync(command.Id);
        if (category == null || category.Type != command.Type)
        {
            throw new NotFoundException(nameof(ArticleCategory), command.Id);
        }

        var slug = command.Slug.Trim().ToLowerInvariant();
        Expression<Func<ArticleCategory, bool>>[] slugFilter =
            [item => item.Type == command.Type && item.Id != command.Id && item.Slug == slug];
        if (await _categoryRepository.AnyAsync(slugFilter))
        {
            throw new InvalidOperationException("Slug danh mục bài viết đã tồn tại.");
        }

        var categories = await _categoryRepository.FindAsync(limit: int.MaxValue);
        EnsureParentHasSameType(command.ParentId, command.Type, categories);
        ArticleCategoryTree.EnsureValidParent(command.Id, command.ParentId, categories);
        if (category.ParentId != command.ParentId)
        {
            await EnsureParentIsNotUsedByArticle(command.ParentId);
        }

        category.Name = command.Name.Trim();
        category.Description = command.Description.Trim();
        category.Slug = slug;
        category.ParentId = command.ParentId;

        await _categoryRepository.UpdateAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static void EnsureParentHasSameType(
        Guid? parentId,
        QuanLyHoaLan.Domain.Enums.ArticleCategoryType type,
        IEnumerable<ArticleCategory> categories)
    {
        if (!parentId.HasValue)
        {
            return;
        }

        var parent = categories.FirstOrDefault(category => category.Id == parentId.Value);
        if (parent == null || parent.Type != type)
        {
            throw new InvalidOperationException("Danh mục cha không tồn tại trong cùng nhóm nội dung.");
        }
    }

    private async Task EnsureParentIsNotUsedByArticle(Guid? parentId)
    {
        if (!parentId.HasValue)
        {
            return;
        }

        Expression<Func<Article, bool>>[] filters =
            [article => article.Categories.Any(category => category.Id == parentId.Value)];
        if (await _articleRepository.AnyAsync(filters))
        {
            throw new InvalidOperationException(
                "Không thể chuyển vào danh mục cha đang được gắn trực tiếp với bài viết.");
        }
    }
}
