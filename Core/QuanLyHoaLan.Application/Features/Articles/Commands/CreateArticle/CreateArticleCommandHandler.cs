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
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IDateTimeService _dateTime;

    public CreateArticleCommandHandler(
        IBaseRepository<Article> articleRepository, 
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        IDateTimeService dateTime)
    {
        _articleRepository = articleRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public async Task<Guid> Handle(CreateArticleCommand command, CancellationToken cancellationToken)
    {
        Expression<Func<Article, bool>>[] filters = new Expression<Func<Article, bool>>[] { x => x.Slug == command.Slug };
        bool slugExists = await _articleRepository.AnyAsync(filters);
        if (slugExists)
        {
            throw new Exception("Slug đã tồn tại.");
        }

        var article = new Article
        {
            Title = command.Title,
            Slug = command.Slug,
            Summary = command.Summary,
            Content = command.Content,
            ThumbnailImageId = command.ThumbnailImageId,
            IsPublished = command.IsPublished,
            PublishedAt = command.IsPublished ? _dateTime.Now : null,
            AuthorId = _currentUser.UserId,
            OrchidIds = command.OrchidIds ?? new List<Guid>(),
            DocumentIds = command.DocumentIds ?? new List<Guid>()
        };

        await _articleRepository.InsertAsync(article, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return article.Id;
    }
}
