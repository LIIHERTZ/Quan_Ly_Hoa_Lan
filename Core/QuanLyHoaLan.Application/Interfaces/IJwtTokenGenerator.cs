using QuanLyHoaLan.Application.DTOs;
using QuanLyHoaLan.Domain.Entities;

namespace QuanLyHoaLan.Application.Interfaces;

public interface IJwtTokenGenerator
{
    JwtAuthResult GenerateToken(User user);
    System.Security.Claims.ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    string GenerateRefreshToken();
}
