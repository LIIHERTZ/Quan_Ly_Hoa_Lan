using MediatR;
using Microsoft.Extensions.Logging;

namespace QuanLyHoaLan.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TRequest> _logger;

    public LoggingBehaviour(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation("QuanLyHoaLan Request Handling: {Name} {@Request}", requestName, request);

        var response = await next();

        _logger.LogInformation("QuanLyHoaLan Request Handled: {Name} {@Response}", requestName, response);

        return response;
    }
}
