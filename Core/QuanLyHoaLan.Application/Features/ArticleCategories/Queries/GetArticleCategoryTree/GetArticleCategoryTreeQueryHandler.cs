using MediatR;
using QuanLyHoaLan.Application.DTOs.ArticleCategory;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.ArticleCategories.Queries.GetArticleCategoryTree;

public class GetArticleCategoryTreeQueryHandler
    : IRequestHandler<GetArticleCategoryTreeQuery, List<ArticleCategoryDto>>
{
    private readonly IBaseRepository<ArticleCategory> _categoryRepository;

    public GetArticleCategoryTreeQueryHandler(IBaseRepository<ArticleCategory> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<List<ArticleCategoryDto>> Handle(
        GetArticleCategoryTreeQuery request,
        CancellationToken cancellationToken)
    {
        System.Linq.Expressions.Expression<Func<ArticleCategory, bool>>[] filters =
            [category => category.Type == request.Type];
        var categories = await _categoryRepository.FindAsync(filters, "Name", limit: int.MaxValue);
        return ArticleCategoryTree.Build(categories);
    }
}
