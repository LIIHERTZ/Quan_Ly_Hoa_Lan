using MediatR;
using QuanLyHoaLan.Application.DTOs;
using QuanLyHoaLan.Application.Interfaces;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Exceptions;
using QuanLyHoaLan.Domain.Interfaces.Repositories;
using System.Linq.Expressions;

namespace QuanLyHoaLan.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResultDto>
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(IJwtTokenGenerator jwtTokenGenerator, IUnitOfWork unitOfWork)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResultDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = _jwtTokenGenerator.GetPrincipalFromExpiredToken(request.Token);
        if (principal == null)
            throw new ValidationException(new Dictionary<string, string[]> { { "Token", new[] { "Invalid token" } } });

        var filters = new Expression<Func<JwtRefreshToken, bool>>[] { x => x.Token == request.RefreshToken };
        var storedToken = await _unitOfWork.Repository<JwtRefreshToken>()
            .FindOneAsync(filters);

        if (storedToken == null)
            throw new ValidationException(new Dictionary<string, string[]> { { "RefreshToken", new[] { "Refresh token does not exist" } } });

        if (storedToken.IsUsed)
            throw new ValidationException(new Dictionary<string, string[]> { { "RefreshToken", new[] { "Refresh token has been used" } } });

        if (storedToken.IsRevoked)
            throw new ValidationException(new Dictionary<string, string[]> { { "RefreshToken", new[] { "Refresh token has been revoked" } } });

        if (storedToken.ExpiryDate < DateTime.UtcNow)
            throw new ValidationException(new Dictionary<string, string[]> { { "RefreshToken", new[] { "Refresh token has expired" } } });

        var jti = principal.Claims.SingleOrDefault(x => x.Type == "jti")?.Value;
        // In a real application, you might want to validate JTI too if you saved it correctly

        storedToken.IsUsed = true;
        await _unitOfWork.Repository<JwtRefreshToken>().UpdateAsync(storedToken);

        var userFilters = new Expression<Func<User, bool>>[] { x => x.Id == storedToken.UserId };
        var userIncludes = new Expression<Func<User, object>>[] { x => x.Role };
        var user = await _unitOfWork.Repository<User>().FindOneAsync(userFilters, null, userIncludes);
        if (user == null)
            throw new ValidationException(new Dictionary<string, string[]> { { "User", new[] { "User does not exist" } } });

        var newAuthResult = _jwtTokenGenerator.GenerateToken(user);

        var newRefreshToken = new JwtRefreshToken
        {
            JwtId = Guid.NewGuid().ToString(),
            Token = newAuthResult.RefreshToken,
            AddedDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddMonths(1),
            IsRevoked = false,
            IsUsed = false,
            UserId = user.Id
        };

        await _unitOfWork.Repository<JwtRefreshToken>().InsertAsync(newRefreshToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResultDto
        {
            Token = newAuthResult.AccessToken,
            RefreshToken = newAuthResult.RefreshToken,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role.Name
        };
    }
}
