using MediatR;
using QuanLyHoaLan.Application.Common.Models;
using QuanLyHoaLan.Application.DTOs.ArticleCategory;
using QuanLyHoaLan.Domain.Enums;

namespace QuanLyHoaLan.Application.Features.ArticleCategories.Queries.GetArticleCategories;

public class GetArticleCategoriesQuery : PagedRequest, IRequest<PaginatedList<ArticleCategoryDto>>
{
    public Guid? ParentId { get; set; }

    internal ArticleCategoryType Type { get; private set; }

    public void UseType(ArticleCategoryType type)
    {
        Type = type;
    }
}
