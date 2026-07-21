using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QuanLyHoaLan.Application.Common.Models;
using QuanLyHoaLan.Application.DTOs.Article;
using QuanLyHoaLan.Application.Interfaces;
using QuanLyHoaLan.Domain.Constants;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Articles.Queries.GetArticles;

public class GetArticlesQueryHandler : IRequestHandler<GetArticlesQuery, PaginatedList<ArticleListDto>>
{
    private readonly IBaseRepository<Article> _articleRepository;
    private readonly IBaseRepository<ArticleCategory> _categoryRepository;
    private readonly ICurrentUser _currentUser;

    public GetArticlesQueryHandler(
        IBaseRepository<Article> articleRepository,
        IBaseRepository<ArticleCategory> categoryRepository,
        ICurrentUser currentUser)
    {
        _articleRepository = articleRepository;
        _categoryRepository = categoryRepository;
        _currentUser = currentUser;
    }

    public async Task<PaginatedList<ArticleListDto>> Handle(GetArticlesQuery request, CancellationToken cancellationToken)
    {
        var articleCategoryIds = await GetArticleCategoryIdsAsync(request);
        var filterByArticleCategory = request.ArticleCategoryId.HasValue;
        var canViewDrafts = _currentUser.HasRole(RoleConstants.Admin);
        
        // Base filters
        var searchLower = request.SearchTerm?.ToLower();
        Expression<Func<Article, bool>> filters = x =>
            (string.IsNullOrEmpty(searchLower) || x.Title.ToLower().Contains(searchLower)) &&
            (canViewDrafts
                ? (!request.IsPublished.HasValue || x.IsPublished == request.IsPublished.Value)
                : x.IsPublished) &&
            (!request.OrchidId.HasValue || x.OrchidIds.Contains(request.OrchidId.Value)) &&
            (!request.CategoryType.HasValue || x.Type == request.CategoryType.Value) &&
            (!filterByArticleCategory || x.Categories.Any(category => articleCategoryIds.Contains(category.Id)));

        var skip = (request.PageNumber - 1) * request.PageSize;

        string orderBy;
        if (string.IsNullOrWhiteSpace(request.SortBy))
        {
            orderBy = "PublishedAt desc, CreatedAt desc";
        }
        else
        {
            var sortBy = request.SortBy.Trim().ToLowerInvariant() switch
            {
                "title" => "Title",
                "createdat" => "CreatedAt",
                _ => "PublishedAt"
            };
            orderBy = request.SortDescending ? $"{sortBy} desc" : sortBy;
        }

        Expression<Func<Article, ArticleListDto>> selector = article => new ArticleListDto
        {
            Id = article.Id,
            Title = article.Title,
            Slug = article.Slug,
            Summary = article.Summary,
            ThumbnailImageId = article.ThumbnailImageId,
            AuthorId = article.AuthorId,
            AuthorName = article.Author.FullName,
            IsPublished = article.IsPublished,
            PublishedAt = article.PublishedAt,
            Type = article.Type,
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

        var result = await _articleRepository.FindProjectedResultAsync(
            [filters],
            orderBy,
            skip,
            request.PageSize,
            selector);

        return PaginatedList<ArticleListDto>.Create(
            result.Items,
            result.TotalCount,
            request.PageNumber,
            request.PageSize);
    }

    private async Task<List<Guid>> GetArticleCategoryIdsAsync(GetArticlesQuery request)
    {
        if (!request.ArticleCategoryId.HasValue)
        {
            return new List<Guid>();
        }

        Expression<Func<ArticleCategory, bool>>[]? filters = request.CategoryType.HasValue
            ? [category => category.Type == request.CategoryType.Value]
            : null;
        var categories = await _categoryRepository.FindAsync(filters, limit: int.MaxValue);

        var selectedCategory = categories.FirstOrDefault(
            category => category.Id == request.ArticleCategoryId.Value);
        if (selectedCategory == null
            || (request.CategoryType.HasValue && selectedCategory.Type != request.CategoryType.Value))
        {
            throw new InvalidOperationException("Danh mục bài viết không tồn tại hoặc đã bị xóa.");
        }

        var result = new HashSet<Guid> { request.ArticleCategoryId.Value };
        if (!request.IncludeDescendants)
        {
            return result.ToList();
        }

        var pending = new Queue<Guid>();
        pending.Enqueue(request.ArticleCategoryId.Value);
        while (pending.Count > 0)
        {
            var parentId = pending.Dequeue();
            foreach (var child in categories.Where(category => category.ParentId == parentId))
            {
                if (result.Add(child.Id))
                {
                    pending.Enqueue(child.Id);
                }
            }
        }

        return result.ToList();
    }
}
