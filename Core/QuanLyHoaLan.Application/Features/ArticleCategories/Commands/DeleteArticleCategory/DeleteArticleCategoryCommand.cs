using MediatR;
using QuanLyHoaLan.Domain.Enums;

namespace QuanLyHoaLan.Application.Features.ArticleCategories.Commands.DeleteArticleCategory;

public record DeleteArticleCategoryCommand(Guid Id, ArticleCategoryType Type) : IRequest<bool>;
