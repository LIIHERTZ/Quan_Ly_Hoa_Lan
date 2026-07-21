using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QuanLyHoaLan.Application.Interfaces;
using QuanLyHoaLan.Domain.Constants;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Exceptions;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Articles.Commands.DeleteArticle;

public class DeleteArticleCommandHandler : IRequestHandler<DeleteArticleCommand, Unit>
{
    private readonly IBaseRepository<Article> _articleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public DeleteArticleCommandHandler(
        IBaseRepository<Article> articleRepository,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser)
    {
        _articleRepository = articleRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(DeleteArticleCommand request, CancellationToken cancellationToken)
    {
        var article = await _articleRepository.FindByIdAsync(request.Id);

        if (article == null)
        {
            throw new NotFoundException(nameof(Article), request.Id);
        }

        var isOwner = article.AuthorId == _currentUser.UserId;
        var isAdmin = _currentUser.HasRole(RoleConstants.Admin);
        if (!isOwner && !isAdmin)
        {
            throw new ForbiddenAccessException("Bạn chỉ có thể xóa bài viết của chính mình.");
        }

        await _articleRepository.DeleteAsync(article, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
