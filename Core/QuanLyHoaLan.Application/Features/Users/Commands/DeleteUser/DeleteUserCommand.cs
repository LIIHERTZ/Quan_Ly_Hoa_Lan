using MediatR;

namespace QuanLyHoaLan.Application.Features.Users.Commands.DeleteUser;

public record DeleteUserCommand(Guid Id) : IRequest;
