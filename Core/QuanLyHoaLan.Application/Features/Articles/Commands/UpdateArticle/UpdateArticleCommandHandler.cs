using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Exceptions;
using QuanLyHoaLan.Domain.Interfaces.Repositories;
using QuanLyHoaLan.Domain.Interfaces.Services;

namespace QuanLyHoaLan.Application.Features.Articles.Commands.UpdateArticle;

public class UpdateArticleCommandHandler : IRequestHandler<UpdateArticleCommand, Unit>
{
    private readonly IBaseRepository<Article> _articleRepository;
    private readonly IBaseRepository<ArticleCategory> _articleCategoryRepository;
    private readonly IBaseRepository<Orchid> _orchidRepository;
    private readonly IBaseRepository<UploadedImage> _imageRepository;
    private readonly IBaseRepository<AppDocument> _documentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeService _dateTime;

    public UpdateArticleCommandHandler(
        IBaseRepository<Article> articleRepository, 
        IBaseRepository<ArticleCategory> articleCategoryRepository,
        IBaseRepository<Orchid> orchidRepository,
        IBaseRepository<UploadedImage> imageRepository,
        IBaseRepository<AppDocument> documentRepository,
        IUnitOfWork unitOfWork,
        IDateTimeService dateTime)
    {
        _articleRepository = articleRepository;
        _articleCategoryRepository = articleCategoryRepository;
        _orchidRepository = orchidRepository;
        _imageRepository = imageRepository;
        _documentRepository = documentRepository;
        _unitOfWork = unitOfWork;
        _dateTime = dateTime;
    }

    public async Task<Unit> Handle(UpdateArticleCommand command, CancellationToken cancellationToken)
    {
        var article = await _articleRepository.FindByIdAsync(command.Id, item => item.Categories);

        if (article == null)
        {
            throw new NotFoundException(nameof(Article), command.Id);
        }

        // Check Slug unique if changed
        var slug = command.Slug.Trim().ToLowerInvariant();
        if (article.Slug != slug)
        {
            Expression<Func<Article, bool>>[] filters = [item => item.Id != command.Id && item.Slug == slug];
            bool slugExists = await _articleRepository.AnyAsync(filters);
            if (slugExists)
            {
                throw new Exception("Slug đã tồn tại.");
            }
        }

        var categories = await ArticleRelationValidator.GetLeafCategoriesAsync(
            command.ArticleCategoryIds,
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

        var wasPublished = article.IsPublished;
        article.Title = command.Title.Trim();
        article.Slug = slug;
        article.Summary = command.Summary.Trim();
        article.Content = command.Content;
        article.ThumbnailImageId = command.ThumbnailImageId;
        article.IsPublished = command.IsPublished;
        article.Categories.Clear();
        foreach (var category in categories)
        {
            article.Categories.Add(category);
        }
        article.OrchidIds = orchidIds;
        article.DocumentIds = documentIds;
        
        if (command.IsPublished && !wasPublished)
        {
            article.PublishedAt = _dateTime.Now;
        }
        else if (!command.IsPublished)
        {
            article.PublishedAt = null;
        }
        
        await _articleRepository.UpdateAsync(article);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
