using System.Linq.Expressions;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Users;

internal static class UserTokenRevocation
{
    public static async Task RevokeAllAsync(
        IBaseRepository<JwtRefreshToken> refreshTokenRepository,
        Guid userId,
        CancellationToken cancellationToken)
    {
        Expression<Func<JwtRefreshToken, bool>>[] filters =
        [
            token => token.UserId == userId && !token.IsRevoked && !token.IsUsed
        ];

        var tokens = await refreshTokenRepository.FindAsync(filters);
        if (tokens.Count == 0)
        {
            return;
        }

        foreach (var token in tokens)
        {
            token.IsRevoked = true;
        }

        await refreshTokenRepository.UpdateRangeAsync(tokens, cancellationToken);
    }
}
