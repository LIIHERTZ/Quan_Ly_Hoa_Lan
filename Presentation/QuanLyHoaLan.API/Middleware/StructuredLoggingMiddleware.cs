using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace QuanLyHoaLan.API.Middleware;

public class StructuredLoggingMiddleware
{
    private static readonly string[] HealthCheckPaths = { "/healthz", "/livez", "/readyz", "/metrics" };
    private readonly RequestDelegate _next;
    private readonly ILogger<StructuredLoggingMiddleware> _logger;

    public StructuredLoggingMiddleware(RequestDelegate next, ILogger<StructuredLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var isHealthCheck = IsHealthCheckRequest(context);
        
        Exception? exception = null;
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            exception = ex;
            throw;
        }
        finally
        {
            stopwatch.Stop();
            var elapsed = stopwatch.ElapsedMilliseconds;
            
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["Latency"] = $"{elapsed}ms",
                ["ErrorMessage"] = exception?.Message ?? ""
            }))
            {
                if (exception != null)
                {
                    _logger.LogError(exception, "HTTP request failed");
                }
                else
                {
                    var statusCode = context.Response.StatusCode;
                    if (statusCode >= 400)
                    {
                        _logger.LogWarning("HTTP request completed with warning status {StatusCode}", statusCode);
                    }
                    else if (!isHealthCheck)
                    {
                        _logger.LogInformation("HTTP request completed successfully in {Elapsed}ms", elapsed);
                    }
                }
            }
        }
    }

    private static bool IsHealthCheckRequest(HttpContext context)
    {
        if (!HttpMethods.IsGet(context.Request.Method))
        {
            return false;
        }

        var pathValue = context.Request.Path.Value;
        if (string.IsNullOrEmpty(pathValue))
        {
            return false;
        }

        return HealthCheckPaths.Any(checkPath => pathValue.StartsWith(checkPath, StringComparison.OrdinalIgnoreCase));
    }
}
