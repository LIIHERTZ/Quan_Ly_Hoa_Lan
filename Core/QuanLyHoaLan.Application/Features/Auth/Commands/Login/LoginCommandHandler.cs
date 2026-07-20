using MediatR;
using QuanLyHoaLan.Application.DTOs;
using QuanLyHoaLan.Application.Interfaces;
using QuanLyHoaLan.Domain.Constants;
using BCrypt.Net;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Interfaces.Repositories;
using System.Linq.Expressions;
using QuanLyHoaLan.Domain.Exceptions;

namespace QuanLyHoaLan.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResultDto>
{
    private readonly IBaseRepository<User> _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(IBaseRepository<User> userRepository, IJwtTokenGenerator jwtTokenGenerator, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResultDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _userRepository.FindOneAsync(
            filters: new Expression<Func<User, bool>>[] { user => user.Email.ToLower() == email },
            includes: new Expression<Func<User, object>>[] { user => user.Role! }
        );
        
        if (user == null || string.IsNullOrEmpty(user.PasswordHash))
        {
            throw new ValidationException(new Dictionary<string, string[]> { { "Email", new[] { "Email hoặc mật khẩu không chính xác." } } });
        }

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            throw new ValidationException(new Dictionary<string, string[]> { { "Email", new[] { "Email hoặc mật khẩu không chính xác." } } });
        }

        var authResult = _jwtTokenGenerator.GenerateToken(user);
        
        var refreshToken = new JwtRefreshToken
        {
            JwtId = Guid.NewGuid().ToString(), // Should match the Jti in token, but we can just save it or extract from token later
            Token = authResult.RefreshToken,
            AddedDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddMonths(1),
            IsRevoked = false,
            IsUsed = false,
            UserId = user.Id
        };
        
        await _unitOfWork.Repository<JwtRefreshToken>().InsertAsync(refreshToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new AuthResultDto
        {
            Token = authResult.AccessToken,
            RefreshToken = authResult.RefreshToken,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role!.Name
        };
    }
}
