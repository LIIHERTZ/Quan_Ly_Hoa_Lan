using MediatR;
using QuanLyHoaLan.Application.DTOs.ArticleCategory;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Exceptions;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.ArticleCategories.Queries.GetArticleCategoryById;

public class GetArticleCategoryByIdQueryHandler
    : IRequestHandler<GetArticleCategoryByIdQuery, ArticleCategoryDto>
{
    private readonly IBaseRepository<ArticleCategory> _categoryRepository;

    public GetArticleCategoryByIdQueryHandler(IBaseRepository<ArticleCategory> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<ArticleCategoryDto> Handle(
        GetArticleCategoryByIdQuery request,
        CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.FindByIdAsync(request.Id, item => item.ParentCategory!);
        if (category == null || category.Type != request.Type)
        {
            throw new NotFoundException(nameof(ArticleCategory), request.Id);
        }

        System.Linq.Expressions.Expression<Func<ArticleCategory, bool>>[] filters =
            [item => item.Type == request.Type];
        var categories = await _categoryRepository.FindAsync(filters, "Name", limit: int.MaxValue);
        var dto = ArticleCategoryTree.Map(category, category.ParentCategory?.Name ?? string.Empty);
        dto.SubCategories = ArticleCategoryTree.Build(categories, category.Id);
        return dto;
    }
}
