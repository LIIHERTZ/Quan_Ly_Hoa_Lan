using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Exceptions;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Articles.Commands.DeleteArticle;

public class DeleteArticleCommandHandler : IRequestHandler<DeleteArticleCommand, Unit>
{
    private readonly IBaseRepository<Article> _articleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteArticleCommandHandler(IBaseRepository<Article> articleRepository, IUnitOfWork unitOfWork)
    {
        _articleRepository = articleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteArticleCommand request, CancellationToken cancellationToken)
    {
        var article = await _articleRepository.FindByIdAsync(request.Id);

        if (article == null)
        {
            throw new NotFoundException(nameof(Article), request.Id);
        }

        await _articleRepository.DeleteAsync(article, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
