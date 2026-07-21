using MediatR;
using QuanLyHoaLan.Application.DTOs.ArticleCategory;
using QuanLyHoaLan.Domain.Enums;

namespace QuanLyHoaLan.Application.Features.ArticleCategories.Queries.GetArticleCategoryTree;

public record GetArticleCategoryTreeQuery(ArticleCategoryType Type) : IRequest<List<ArticleCategoryDto>>;
