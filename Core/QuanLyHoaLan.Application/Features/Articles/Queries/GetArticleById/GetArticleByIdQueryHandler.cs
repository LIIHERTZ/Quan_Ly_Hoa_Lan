using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QuanLyHoaLan.Application.DTOs.Article;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Exceptions;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Articles.Queries.GetArticleById;

public class GetArticleByIdQueryHandler : IRequestHandler<GetArticleByIdQuery, ArticleDto>
{
    private readonly IBaseRepository<Article> _articleRepository;

    public GetArticleByIdQueryHandler(IBaseRepository<Article> articleRepository)
    {
        _articleRepository = articleRepository;
    }

    public async Task<ArticleDto> Handle(GetArticleByIdQuery request, CancellationToken cancellationToken)
    {
        var includes = new Expression<Func<Article, object>>[] 
        { 
            x => x.Author
        };

        var article = await _articleRepository.FindByIdAsync(request.Id, includes);

        if (article == null)
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
            OrchidIds = article.OrchidIds,
            DocumentIds = article.DocumentIds
        };
    }
}
