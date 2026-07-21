using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using QuanLyHoaLan.Application.Common.Models;
using QuanLyHoaLan.Domain.Constants;
using QuanLyHoaLan.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace QuanLyHoaLan.API.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, errorCode, message) = exception switch
        {
            ValidationException validationException => (StatusCodes.Status400BadRequest, ErrorCodes.BAD_REQUEST, "Dữ liệu không hợp lệ."),
            ForbiddenAccessException => (StatusCodes.Status403Forbidden, ErrorCodes.FORBIDDEN, "Bạn không có quyền thực hiện hành động này."),
            UnauthorizedException unauthorizedException => (StatusCodes.Status401Unauthorized, ErrorCodes.UNAUTHORIZED, unauthorizedException.Message),
            NotFoundException notFoundException => (StatusCodes.Status404NotFound, ErrorCodes.NOT_FOUND, notFoundException.Message),
            InvalidOperationException invalidOperationException => (StatusCodes.Status400BadRequest, ErrorCodes.BAD_REQUEST, invalidOperationException.Message),
            DbUpdateException { InnerException: PostgresException postgresException }
                when postgresException.SqlState == PostgresErrorCodes.CheckViolation
                    || postgresException.SqlState == PostgresErrorCodes.ForeignKeyViolation
                => (StatusCodes.Status400BadRequest, ErrorCodes.BAD_REQUEST, "Dữ liệu vi phạm ràng buộc hệ thống."),
            DbUpdateException { InnerException: PostgresException postgresException }
                when postgresException.SqlState == PostgresErrorCodes.UniqueViolation
                => (StatusCodes.Status409Conflict, ErrorCodes.CONFLICT, "Dữ liệu đã tồn tại."),
            _ => (StatusCodes.Status500InternalServerError, ErrorCodes.SERVER_ERROR, "Đã xảy ra lỗi hệ thống cục bộ. Vui lòng thử lại sau.")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var validationErrors = (exception as ValidationException)?.Errors;
            
        var response = ApiResponse<object>.Fail(
            message: message,
            errorCode: errorCode,
            validationErrors: validationErrors
        );

        return context.Response.WriteAsJsonAsync(response);
    }
}
