using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QuanLyHoaLan.Application.Interfaces;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Interfaces.Repositories;
using QuanLyHoaLan.Domain.Interfaces.Services;

namespace QuanLyHoaLan.Application.Features.Articles.Commands.CreateArticle;

public class CreateArticleCommandHandler : IRequestHandler<CreateArticleCommand, Guid>
{
    private readonly IBaseRepository<Article> _articleRepository;
    private readonly IBaseRepository<ArticleCategory> _articleCategoryRepository;
    private readonly IBaseRepository<Orchid> _orchidRepository;
    private readonly IBaseRepository<UploadedImage> _imageRepository;
    private readonly IBaseRepository<AppDocument> _documentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IDateTimeService _dateTime;

    public CreateArticleCommandHandler(
        IBaseRepository<Article> articleRepository, 
        IBaseRepository<ArticleCategory> articleCategoryRepository,
        IBaseRepository<Orchid> orchidRepository,
        IBaseRepository<UploadedImage> imageRepository,
        IBaseRepository<AppDocument> documentRepository,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        IDateTimeService dateTime)
    {
        _articleRepository = articleRepository;
        _articleCategoryRepository = articleCategoryRepository;
        _orchidRepository = orchidRepository;
        _imageRepository = imageRepository;
        _documentRepository = documentRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public async Task<Guid> Handle(CreateArticleCommand command, CancellationToken cancellationToken)
    {
        var slug = command.Slug.Trim().ToLowerInvariant();
        Expression<Func<Article, bool>>[] filters = [article => article.Slug == slug];
        bool slugExists = await _articleRepository.AnyAsync(filters);
        if (slugExists)
        {
            throw new Exception("Slug đã tồn tại.");
        }

        var articleType = command.Type!.Value;
        var categories = await ArticleRelationValidator.GetLeafCategoriesAsync(
            command.ArticleCategoryIds,
            articleType,
            _articleCategoryRepository);
        var orchidIds = await ArticleRelationValidator.EnsureIdsExistAsync(
            command.OrchidIds,
            _orchidRepository,
            "Orchid");
        var documentIds = await ArticleRelationValidator.EnsureIdsExistAsync(
            command.DocumentIds,
            _documentRepository,
            "tài liệu");
        await ArticleRelationValidator.EnsureOptionalIdExistsAsync(
            command.ThumbnailImageId,
            _imageRepository,
            "Ảnh đại diện");

        var article = new Article
        {
            Title = command.Title.Trim(),
            Slug = slug,
            Summary = command.Summary?.Trim() ?? string.Empty,
            Content = command.Content,
            ThumbnailImageId = command.ThumbnailImageId,
            IsPublished = command.IsPublished,
            PublishedAt = command.IsPublished ? _dateTime.Now : null,
            Type = articleType,
            AuthorId = _currentUser.UserId,
            Categories = categories,
            OrchidIds = orchidIds,
            DocumentIds = documentIds
        };

        await _articleRepository.InsertAsync(article, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return article.Id;
    }
}
