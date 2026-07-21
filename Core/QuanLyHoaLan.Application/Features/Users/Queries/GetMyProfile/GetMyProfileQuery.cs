using MediatR;
using QuanLyHoaLan.Application.DTOs.User;

namespace QuanLyHoaLan.Application.Features.Users.Queries.GetMyProfile;

public record GetMyProfileQuery : IRequest<UserDto>;
