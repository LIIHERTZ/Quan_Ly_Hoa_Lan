using Microsoft.Extensions.DependencyInjection;

namespace QuanLyHoaLan.API.Extensions;

public static class CorsServiceExtensions
{
    public const string DefaultCorsPolicy = "DefaultCorsPolicy";

    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(DefaultCorsPolicy, builder =>
            {
                builder.WithOrigins(
                           "http://localhost:3000",
                           "https://localhost:3000",
                           "https://hoa-lan.vercel.app")
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

        return services;
    }
}
