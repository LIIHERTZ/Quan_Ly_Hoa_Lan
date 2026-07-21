using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QuanLyHoaLan.Application.DTOs.Article;
using QuanLyHoaLan.Application.Interfaces;
using QuanLyHoaLan.Domain.Constants;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Exceptions;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Articles.Queries.GetArticleById;

public class GetArticleByIdQueryHandler : IRequestHandler<GetArticleByIdQuery, ArticleDto>
{
    private readonly IBaseRepository<Article> _articleRepository;
    private readonly ICurrentUser _currentUser;

    public GetArticleByIdQueryHandler(
        IBaseRepository<Article> articleRepository,
        ICurrentUser currentUser)
    {
        _articleRepository = articleRepository;
        _currentUser = currentUser;
    }

    public async Task<ArticleDto> Handle(GetArticleByIdQuery request, CancellationToken cancellationToken)
    {
        var includes = new Expression<Func<Article, object>>[] 
        { 
            x => x.Author,
            x => x.Categories
        };

        var article = await _articleRepository.FindByIdAsync(request.Id, includes);

        if (article == null || (!article.IsPublished && !_currentUser.HasRole(RoleConstants.Admin)))
        {
            throw new NotFoundException(nameof(Article), request.Id);
        }

        return new ArticleDto
        {
            Id = article.Id,
            Title = article.Title,
            Slug = article.Slug,
            Summary = article.Summary,
            Content = article.Content,
            ThumbnailImageId = article.ThumbnailImageId,
            AuthorId = article.AuthorId,
            AuthorName = article.Author?.FullName ?? string.Empty,
            IsPublished = article.IsPublished,
            PublishedAt = article.PublishedAt,
            Categories = article.Categories
                .Where(category => !category.IsDeleted)
                .Select(category => new SimpleArticleCategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Slug = category.Slug,
                    Type = category.Type
                }).ToList(),
            OrchidIds = article.OrchidIds,
            DocumentIds = article.DocumentIds
        };
    }
}
