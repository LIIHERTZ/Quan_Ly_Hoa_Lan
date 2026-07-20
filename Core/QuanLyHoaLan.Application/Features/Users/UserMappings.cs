using QuanLyHoaLan.Application.DTOs.User;
using QuanLyHoaLan.Domain.Entities;

namespace QuanLyHoaLan.Application.Features.Users;

internal static class UserMappings
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            AvatarUrl = user.AvatarUrl,
            RoleId = user.RoleId,
            RoleCode = user.Role?.Code ?? string.Empty,
            RoleName = user.Role?.Name ?? string.Empty,
            HasPassword = !string.IsNullOrEmpty(user.PasswordHash),
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}
