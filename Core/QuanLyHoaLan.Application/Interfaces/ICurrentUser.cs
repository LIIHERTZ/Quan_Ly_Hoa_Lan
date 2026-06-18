namespace QuanLyHoaLan.Application.Interfaces;

public interface ICurrentUser
{
    Guid UserId { get; }
    string? UserIdString { get; }
    string? PhoneNumber { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    string? Role { get; }
    IEnumerable<string> Roles { get; }
    IEnumerable<string> Policies { get; }
    bool HasRole(string role);
    bool HasPolicy(string policy);
}
