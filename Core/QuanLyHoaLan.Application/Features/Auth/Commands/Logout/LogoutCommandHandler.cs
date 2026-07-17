using MediatR;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Interfaces.Repositories;
using System.Linq.Expressions;

namespace QuanLyHoaLan.Application.Features.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public LogoutCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var filters = new Expression<Func<JwtRefreshToken, bool>>[] { x => x.Token == request.RefreshToken };
        var storedToken = await _unitOfWork.Repository<JwtRefreshToken>().FindOneAsync(filters);

        if (storedToken != null)
        {
            storedToken.IsRevoked = true;
            await _unitOfWork.Repository<JwtRefreshToken>().UpdateAsync(storedToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return true;
    }
}
