using MediatR;
using QuanLyHoaLan.Application.Interfaces;

namespace QuanLyHoaLan.Application.Common.Behaviours;

public class CurrentUserBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ICurrentUser _currentUser;

    public CurrentUserBehaviour(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is ICurrentUserRequest currentUserRequest)
        {
            currentUserRequest.UserId = _currentUser.UserId;
        }

        return await next();
    }
}

public interface ICurrentUserRequest
{
    Guid UserId { get; set; }
}
