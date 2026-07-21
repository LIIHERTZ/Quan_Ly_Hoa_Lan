using MediatR;
using QuanLyHoaLan.Application.Common.Models;
using QuanLyHoaLan.Application.DTOs.Article;
using System;
using QuanLyHoaLan.Domain.Enums;

namespace QuanLyHoaLan.Application.Features.Articles.Queries.GetArticles;

public class GetArticlesQuery : PagedRequest, IRequest<PaginatedList<ArticleListDto>>
{
    public Guid? OrchidId { get; set; }
    public Guid? ArticleCategoryId { get; set; }
    public bool IncludeDescendants { get; set; } = true;
    public bool? IsPublished { get; set; }

    internal ArticleCategoryType? CategoryType { get; private set; }

    public void UseCategoryType(ArticleCategoryType type)
    {
        CategoryType = type;
    }
}
