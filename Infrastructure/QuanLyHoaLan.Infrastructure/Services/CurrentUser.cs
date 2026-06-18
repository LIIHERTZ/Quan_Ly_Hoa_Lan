using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using QuanLyHoaLan.Application.Interfaces;

namespace QuanLyHoaLan.Infrastructure.Services;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var userIdString = GetClaimValue("userId") ?? GetClaimValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdString, out var userId) ? userId : Guid.Empty;
        }
    }

    public string? UserIdString => GetClaimValue("userId") ?? GetClaimValue(ClaimTypes.NameIdentifier);

    public string? PhoneNumber => GetClaimValue("phone") ?? GetClaimValue(ClaimTypes.MobilePhone);

    public string? Email => GetClaimValue(ClaimTypes.Email);

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public string? Role => GetClaimValue("role") ?? GetClaimValue(ClaimTypes.Role);

    public IEnumerable<string> Roles => GetClaimValues(ClaimTypes.Role);

    public IEnumerable<string> Policies => GetClaimValues("policy");

    public bool HasRole(string role)
    {
        return Roles.Contains(role, StringComparer.OrdinalIgnoreCase);
    }

    public bool HasPolicy(string policy)
    {
        return Policies.Contains(policy, StringComparer.OrdinalIgnoreCase);
    }

    private string? GetClaimValue(string claimType)
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(claimType)?.Value;
    }

    private IEnumerable<string> GetClaimValues(string claimType)
    {
        return _httpContextAccessor.HttpContext?.User?.FindAll(claimType)?.Select(c => c.Value) ?? [];
    }
}
