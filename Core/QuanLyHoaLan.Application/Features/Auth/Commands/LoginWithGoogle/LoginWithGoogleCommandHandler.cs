using MediatR;
using QuanLyHoaLan.Application.DTOs;
using QuanLyHoaLan.Application.Interfaces;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Constants;
using QuanLyHoaLan.Domain.Interfaces.Repositories;
using System.Linq.Expressions;
using QuanLyHoaLan.Domain.Exceptions;

namespace QuanLyHoaLan.Application.Features.Auth.Commands.LoginWithGoogle;

public class LoginWithGoogleCommandHandler : IRequestHandler<LoginWithGoogleCommand, AuthResultDto>
{
    private readonly IGoogleAuthService _googleAuthService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public LoginWithGoogleCommandHandler(
        IGoogleAuthService googleAuthService,
        IJwtTokenGenerator jwtTokenGenerator,
        IUnitOfWork unitOfWork)
    {
        _googleAuthService = googleAuthService;
        _jwtTokenGenerator = jwtTokenGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResultDto> Handle(LoginWithGoogleCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate Google Token
        var googleUser = await _googleAuthService.ValidateAsync(request.IdToken);
        if (googleUser == null)
        {
            throw new UnauthorizedException("Invalid Google Token");
        }

        // 2. Check if user exists
        var userRepository = _unitOfWork.Repository<User>();
        var roleRepository = _unitOfWork.Repository<Role>();
        var email = googleUser.Email.Trim().ToLowerInvariant();

        var user = await userRepository.FindOneAsync(
            filters: new Expression<Func<User, bool>>[] { entity => entity.Email.ToLower() == email },
            includes: new Expression<Func<User, object>>[] { entity => entity.Role! }
        );

        // 3. If not exists, create a new user
        if (user == null)
        {
            Expression<Func<User, bool>>[] deletedUserFilters = [entity => entity.Email.ToLower() == email];
            if (await userRepository.AnyAsync(deletedUserFilters, includeDeleted: true))
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    ["Email"] = ["Tài khoản sử dụng email này đã bị xóa. Vui lòng liên hệ quản trị viên."]
                });
            }

            var defaultRole = await roleRepository.FindOneAsync(
                filters: new Expression<Func<Role, bool>>[] { role => role.Code == RoleConstants.UserCode && role.IsActive }
            );

            if (defaultRole == null)
            {
                throw new NotFoundException("Default role not found in the database.");
            }

            user = new User
            {
                Email = email,
                FullName = googleUser.Name,
                AvatarUrl = googleUser.Picture,
                GoogleId = googleUser.GoogleId,
                RoleId = defaultRole.Id,
                Role = defaultRole
            };
            
            await userRepository.InsertAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }
        else if (string.IsNullOrEmpty(user.GoogleId))
        {
            // Link Google account to existing email account
            user.GoogleId = googleUser.GoogleId;
            if (string.IsNullOrEmpty(user.AvatarUrl))
            {
                user.AvatarUrl = googleUser.Picture;
            }
            await userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }

        // 4. Generate system JWT Token
        var authResult = _jwtTokenGenerator.GenerateToken(user);
        
        var refreshToken = new JwtRefreshToken
        {
            JwtId = Guid.NewGuid().ToString(),
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
            Role = user.Role?.Name ?? RoleConstants.User
        };
    }
}
