using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using FluentValidation;
using QuanLyHoaLan.Application.Interfaces;
using QuanLyHoaLan.Infrastructure.Authentication;
using QuanLyHoaLan.API.Extensions;
using Microsoft.EntityFrameworkCore;
using QuanLyHoaLan.Infrastructure.Persistence;
using QuanLyHoaLan.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new QuanLyHoaLan.API.Serialization.DateTimeUtcConverter());
        options.JsonSerializerOptions.Converters.Add(new QuanLyHoaLan.API.Serialization.NullableDateTimeUtcConverter());
    });

// 1. Register Application MediatR
builder.Services.AddValidatorsFromAssembly(typeof(QuanLyHoaLan.Application.Features.Auth.Commands.LoginWithGoogle.LoginWithGoogleCommand).Assembly);

builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(QuanLyHoaLan.Application.Features.Auth.Commands.LoginWithGoogle.LoginWithGoogleCommand).Assembly);
    cfg.AddBehavior(typeof(MediatR.IPipelineBehavior<,>), typeof(QuanLyHoaLan.Application.Common.Behaviours.UnhandledExceptionBehaviour<,>));
    cfg.AddBehavior(typeof(MediatR.IPipelineBehavior<,>), typeof(QuanLyHoaLan.Application.Common.Behaviours.AuthorizationBehaviour<,>));
    cfg.AddBehavior(typeof(MediatR.IPipelineBehavior<,>), typeof(QuanLyHoaLan.Application.Common.Behaviours.CurrentUserBehaviour<,>));
    cfg.AddBehavior(typeof(MediatR.IPipelineBehavior<,>), typeof(QuanLyHoaLan.Application.Common.Behaviours.ValidationBehaviour<,>));
    cfg.AddBehavior(typeof(MediatR.IPipelineBehavior<,>), typeof(QuanLyHoaLan.Application.Common.Behaviours.PerformanceBehaviour<,>));
    cfg.AddBehavior(typeof(MediatR.IPipelineBehavior<,>), typeof(QuanLyHoaLan.Application.Common.Behaviours.UnitOfWorkBehavior<,>));
    cfg.AddBehavior(typeof(MediatR.IPipelineBehavior<,>), typeof(QuanLyHoaLan.Application.Common.Behaviours.LoggingBehaviour<,>));
});

// 2. Register Infrastructure Services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<QuanLyHoaLan.Application.Interfaces.ICurrentUser, QuanLyHoaLan.Infrastructure.Services.CurrentUser>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();
builder.Services.AddScoped(typeof(QuanLyHoaLan.Domain.Interfaces.Repositories.IBaseRepository<>), typeof(QuanLyHoaLan.Infrastructure.Persistence.Repositories.BaseRepository<>));
builder.Services.AddScoped<QuanLyHoaLan.Domain.Interfaces.Repositories.IUnitOfWork, QuanLyHoaLan.Infrastructure.Persistence.Repositories.UnitOfWork>();
builder.Services.AddScoped<QuanLyHoaLan.Domain.Interfaces.Services.IDateTimeService, QuanLyHoaLan.Infrastructure.Services.DateTimeService>();

// 2.2 Cloudinary Settings & Service
builder.Services.Configure<QuanLyHoaLan.Infrastructure.Settings.CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));
builder.Services.AddScoped<QuanLyHoaLan.Application.Interfaces.Services.IImageService, QuanLyHoaLan.Infrastructure.Services.CloudinaryService>();
builder.Services.AddScoped<QuanLyHoaLan.Application.Interfaces.Services.IDocumentService, QuanLyHoaLan.Infrastructure.Services.CloudinaryService>();

// 2.5 Register Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// 3. Setup JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"] ?? throw new ArgumentNullException("JwtSettings:Secret");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// 4. Rate Limiting & CORS & Swagger
builder.Services.AddCorsConfiguration();
builder.Services.AddRateLimitingConfiguration();
builder.Services.AddSwaggerConfiguration();

var app = builder.Build();

app.UseMiddleware<QuanLyHoaLan.API.Middleware.SecurityHeadersMiddleware>();
app.UseMiddleware<QuanLyHoaLan.API.Middleware.StructuredLoggingMiddleware>();
app.UseMiddleware<QuanLyHoaLan.API.Middleware.ErrorHandlingMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();
    await QuanLyHoaLan.Infrastructure.Persistence.Seeders.DatabaseSeeder.SeedAsync(context);
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseReDoc(options =>
{
    options.DocumentTitle = "QuanLyHoaLan API Documentation";
    options.SpecUrl = "/swagger/api/swagger.json";
    options.RoutePrefix = "redocs";
});

app.UseHttpsRedirection();

app.UseCors(QuanLyHoaLan.API.Extensions.CorsServiceExtensions.DefaultCorsPolicy);
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
