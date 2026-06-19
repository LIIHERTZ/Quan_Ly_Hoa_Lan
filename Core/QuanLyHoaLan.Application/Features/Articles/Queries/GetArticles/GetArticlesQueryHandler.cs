using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QuanLyHoaLan.Application.Common.Models;
using QuanLyHoaLan.Application.DTOs.Article;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Articles.Queries.GetArticles;

public class GetArticlesQueryHandler : IRequestHandler<GetArticlesQuery, PaginatedList<ArticleDto>>
{
    private readonly IBaseRepository<Article> _articleRepository;

    public GetArticlesQueryHandler(IBaseRepository<Article> articleRepository)
    {
        _articleRepository = articleRepository;
    }

    public async Task<PaginatedList<ArticleDto>> Handle(GetArticlesQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Article, bool>>? filters = null;
        
        // Base filters
        var searchLower = request.SearchTerm?.ToLower();
        filters = x => 
            (string.IsNullOrEmpty(searchLower) || x.Title.ToLower().Contains(searchLower)) &&
            (!request.IsPublished.HasValue || x.IsPublished == request.IsPublished.Value) &&
            (!request.OrchidId.HasValue || x.OrchidIds.Contains(request.OrchidId.Value));

        var skip = (request.PageNumber - 1) * request.PageSize;

        string orderBy = "PublishedAt desc, CreatedAt desc";
        if (!string.IsNullOrEmpty(request.SortBy))
        {
            orderBy = request.SortBy;
            if (request.SortDescending)
            {
                orderBy += " desc";
            }
        }

        var includes = new Expression<Func<Article, object>>[] 
        { 
            x => x.Author
        };

        var result = await _articleRepository.FindResultAsync(new[] { filters }, orderBy, skip, request.PageSize, includes);

        var dtos = result.Items.Select(article => new ArticleDto
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
        }).ToList();

        return PaginatedList<ArticleDto>.Create(dtos, result.TotalCount, request.PageNumber, request.PageSize);
    }
}
