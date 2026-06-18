using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace QuanLyHoaLan.API.Middleware;

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SecurityHeadersMiddleware> _logger;

    public SecurityHeadersMiddleware(RequestDelegate next, ILogger<SecurityHeadersMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        AddSecurityHeaders(context);
        
        await _next(context);
    }

    private void AddSecurityHeaders(HttpContext context)
    {
        var response = context.Response;
        
        if (!response.Headers.ContainsKey("X-Content-Type-Options"))
        {
            response.Headers.Append("X-Content-Type-Options", "nosniff");
        }
        
        if (!response.Headers.ContainsKey("X-Frame-Options"))
        {
            response.Headers.Append("X-Frame-Options", "DENY");
        }
        
        if (!response.Headers.ContainsKey("Referrer-Policy"))
        {
            response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
        }
        
        if (!response.Headers.ContainsKey("X-XSS-Protection"))
        {
            response.Headers.Append("X-XSS-Protection", "1; mode=block");
        }
        
        if (!response.Headers.ContainsKey("Content-Security-Policy"))
        {
            response.Headers.Append("Content-Security-Policy", "default-src 'self'; frame-ancestors 'none';");
        }
        
        if (!response.Headers.ContainsKey("Permissions-Policy"))
        {
            response.Headers.Append("Permissions-Policy", "geolocation=(), microphone=(), camera=()");
        }
        
        if (!response.Headers.ContainsKey("Cache-Control"))
        {
            response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
        }
        
        if (!response.Headers.ContainsKey("Pragma"))
        {
            response.Headers.Append("Pragma", "no-cache");
        }
        
        _logger.LogDebug("Security headers added to response for {RequestPath}", context.Request.Path);
    }
}
