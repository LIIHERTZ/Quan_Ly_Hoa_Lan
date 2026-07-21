using System.Linq.Expressions;
using MediatR;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Exceptions;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.ArticleCategories.Commands.DeleteArticleCategory;

public class DeleteArticleCategoryCommandHandler : IRequestHandler<DeleteArticleCategoryCommand, bool>
{
    private readonly IBaseRepository<ArticleCategory> _categoryRepository;
    private readonly IBaseRepository<Article> _articleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteArticleCategoryCommandHandler(
        IBaseRepository<ArticleCategory> categoryRepository,
        IBaseRepository<Article> articleRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _articleRepository = articleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteArticleCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.FindByIdAsync(command.Id);
        if (category == null || category.Type != command.Type)
        {
            throw new NotFoundException(nameof(ArticleCategory), command.Id);
        }

        Expression<Func<ArticleCategory, bool>>[] childFilter = [item => item.ParentId == command.Id];
        if (await _categoryRepository.AnyAsync(childFilter))
        {
            throw new InvalidOperationException("Không thể xóa danh mục đang có danh mục con.");
        }

        Expression<Func<Article, bool>>[] articleFilter =
            [article => article.Categories.Any(item => item.Id == command.Id)];
        if (await _articleRepository.AnyAsync(articleFilter))
        {
            throw new InvalidOperationException("Không thể xóa danh mục đang được bài viết sử dụng.");
        }

        await _categoryRepository.DeleteAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
