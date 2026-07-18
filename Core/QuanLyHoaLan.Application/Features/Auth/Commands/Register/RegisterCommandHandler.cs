using MediatR;
using QuanLyHoaLan.Application.DTOs;
using QuanLyHoaLan.Application.Interfaces;
using QuanLyHoaLan.Domain.Constants;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Exceptions;
using QuanLyHoaLan.Domain.Interfaces.Repositories;
using System.Linq.Expressions;
using BCrypt.Net;

namespace QuanLyHoaLan.Application.Features.Auth.Commands.Register;

// Handles the registration logic: check email existence, hash password, save user, generate tokens.
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResultDto>
{
    private readonly IBaseRepository<User> _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(
        IBaseRepository<User> userRepository,
        IJwtTokenGenerator jwtTokenGenerator,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResultDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // 1. Check if email already exists
        var existingUser = await _userRepository.FindOneAsync(
            filters: new Expression<Func<User, bool>>[] { u => u.Email == request.Email }
        );

        if (existingUser != null)
        {
            throw new ValidationException(new Dictionary<string, string[]> { { "Email", new[] { "Email đã được sử dụng." } } });
        }

        // 2. Fetch default Role (User)
        var roleRepository = _unitOfWork.Repository<Role>();
        var defaultRole = await roleRepository.FindOneAsync(
            filters: new Expression<Func<Role, bool>>[] { r => r.Name == RoleConstants.User }
        );

        if (defaultRole == null)
        {
            throw new NotFoundException("Default role not found in the database.");
        }

        // 3. Create new user and hash password
        var user = new User
        {
            Email = request.Email,
            FullName = request.FullName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            RoleId = defaultRole.Id
        };

        await _userRepository.InsertAsync(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        user.Role = defaultRole;

        // 4. Generate Tokens
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
            Role = user.Role.Name
        };
    }
}
