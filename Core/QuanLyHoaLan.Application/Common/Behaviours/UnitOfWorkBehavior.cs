using MediatR;
using Microsoft.Extensions.Logging;
using QuanLyHoaLan.Application.Common.Attributes;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Common.Behaviours;

public class UnitOfWorkBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UnitOfWorkBehavior<TRequest, TResponse>> _logger;

    public UnitOfWorkBehavior(IUnitOfWork unitOfWork, ILogger<UnitOfWorkBehavior<TRequest, TResponse>> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!request.GetType().GetCustomAttributes(typeof(EnableUnitOfWorkAttribute), true).Any())
        {
            return await next();
        }

        var requestName = typeof(TRequest).Name;
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var response = await next();
            await _unitOfWork.CommitTransactionAsync();
            _logger.LogInformation("Transaction committed for {RequestName}", requestName);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed for {RequestName}. Rolling back...", requestName);
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
