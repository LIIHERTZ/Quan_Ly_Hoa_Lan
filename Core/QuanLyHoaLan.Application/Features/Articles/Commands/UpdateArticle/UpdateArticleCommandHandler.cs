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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeService _dateTime;

    public UpdateArticleCommandHandler(
        IBaseRepository<Article> articleRepository, 
        IUnitOfWork unitOfWork,
        IDateTimeService dateTime)
    {
        _articleRepository = articleRepository;
        _unitOfWork = unitOfWork;
        _dateTime = dateTime;
    }

    public async Task<Unit> Handle(UpdateArticleCommand command, CancellationToken cancellationToken)
    {
        var article = await _articleRepository.FindByIdAsync(command.Id);

        if (article == null)
        {
            throw new NotFoundException(nameof(Article), command.Id);
        }

        // Check Slug unique if changed
        if (article.Slug != command.Slug)
        {
            Expression<Func<Article, bool>>[] filters = new Expression<Func<Article, bool>>[] { x => x.Slug == command.Slug };
            bool slugExists = await _articleRepository.AnyAsync(filters);
            if (slugExists)
            {
                throw new Exception("Slug đã tồn tại.");
            }
        }

        article.Title = command.Title;
        article.Slug = command.Slug;
        article.Summary = command.Summary;
        article.Content = command.Content;
        article.ThumbnailImageId = command.ThumbnailImageId;
        article.IsPublished = command.IsPublished;
        article.OrchidIds = command.OrchidIds ?? new List<Guid>();
        article.DocumentIds = command.DocumentIds ?? new List<Guid>();
        
        if (command.IsPublished && !article.IsPublished)
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
