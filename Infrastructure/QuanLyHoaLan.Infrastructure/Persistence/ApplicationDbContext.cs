using Microsoft.EntityFrameworkCore;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Constants;
using QuanLyHoaLan.Application.Interfaces;
using QuanLyHoaLan.Domain.Interfaces.Services;
using MediatR;
using QuanLyHoaLan.Infrastructure.Persistence.Interceptors;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace QuanLyHoaLan.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    private readonly ICurrentUser _currentUser;
    private readonly IDateTimeService _dateTime;
    private readonly IMediator _mediator;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUser currentUser,
        IDateTimeService dateTime,
        IMediator mediator) : base(options)
    {
        _currentUser = currentUser;
        _dateTime = dateTime;
        _mediator = mediator;
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<JwtRefreshToken> JwtRefreshTokens { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Orchid> Orchids { get; set; } = null!;
    public DbSet<Article> Articles { get; set; } = null!;
    public DbSet<ArticleCategory> ArticleCategories { get; set; } = null!;
    public DbSet<DiscussionPost> DiscussionPosts => Set<DiscussionPost>();
    public DbSet<DiscussionComment> DiscussionComments => Set<DiscussionComment>();
    public DbSet<UploadedImage> UploadedImages { get; set; } = null!;
    public DbSet<AppDocument> AppDocuments => Set<AppDocument>();
    public DbSet<DocumentCategory> DocumentCategories => Set<DocumentCategory>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Fallback if not configured from Program.cs
        }

        optionsBuilder.AddInterceptors(
            new AuditableEntityInterceptor(_currentUser, _dateTime),
            new DispatchDomainEventsInterceptor(_mediator, new Logger<DispatchDomainEventsInterceptor>(new LoggerFactory()))
        );

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
