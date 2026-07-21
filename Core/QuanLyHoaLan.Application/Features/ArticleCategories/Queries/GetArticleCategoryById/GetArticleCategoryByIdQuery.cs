using MediatR;
using QuanLyHoaLan.Application.DTOs.ArticleCategory;
using QuanLyHoaLan.Domain.Enums;

namespace QuanLyHoaLan.Application.Features.ArticleCategories.Queries.GetArticleCategoryById;

public record GetArticleCategoryByIdQuery(Guid Id, ArticleCategoryType Type) : IRequest<ArticleCategoryDto>;
