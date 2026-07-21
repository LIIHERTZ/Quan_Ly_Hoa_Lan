using System.Linq.Expressions;
using MediatR;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.ArticleCategories.Commands.CreateArticleCategory;

public class CreateArticleCategoryCommandHandler : IRequestHandler<CreateArticleCategoryCommand, Guid>
{
    private readonly IBaseRepository<ArticleCategory> _categoryRepository;
    private readonly IBaseRepository<Article> _articleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateArticleCategoryCommandHandler(
        IBaseRepository<ArticleCategory> categoryRepository,
        IBaseRepository<Article> articleRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _articleRepository = articleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateArticleCategoryCommand command, CancellationToken cancellationToken)
    {
        var slug = command.Slug.Trim().ToLowerInvariant();
        Expression<Func<ArticleCategory, bool>>[] slugFilter =
            [category => category.Type == command.Type && category.Slug == slug];
        if (await _categoryRepository.AnyAsync(slugFilter))
        {
            throw new InvalidOperationException("Slug danh mục bài viết đã tồn tại.");
        }

        var categories = await _categoryRepository.FindAsync(limit: int.MaxValue);
        EnsureParentHasSameType(command.ParentId, command.Type, categories);
        ArticleCategoryTree.EnsureValidParent(Guid.NewGuid(), command.ParentId, categories);
        await EnsureParentIsNotUsedByArticle(command.ParentId);

        var category = new ArticleCategory
        {
            Name = command.Name.Trim(),
            Description = command.Description?.Trim() ?? string.Empty,
            Slug = slug,
            Type = command.Type,
            ParentId = command.ParentId
        };

        await _categoryRepository.InsertAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return category.Id;
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
                "Không thể thêm danh mục con vì danh mục cha đang được gắn trực tiếp với bài viết.");
        }
    }
}
