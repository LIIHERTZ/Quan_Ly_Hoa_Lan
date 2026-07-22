using System.Linq.Expressions;
using MediatR;
using QuanLyHoaLan.Application.DTOs.Dashboard;
using QuanLyHoaLan.Application.DTOs.Orchid;
using QuanLyHoaLan.Application.Interfaces;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Enums;
using QuanLyHoaLan.Domain.Exceptions;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Dashboard.Queries.GetDashboardOverview;

public class GetDashboardOverviewQueryHandler
    : IRequestHandler<GetDashboardOverviewQuery, DashboardOverviewDto>
{
    private const int RecentOrchidLimit = 3;

    private readonly IBaseRepository<Orchid> _orchidRepository;
    private readonly IBaseRepository<AppDocument> _documentRepository;
    private readonly IBaseRepository<Article> _articleRepository;
    private readonly IBaseRepository<User> _userRepository;
    private readonly IBaseRepository<UploadedImage> _imageRepository;
    private readonly ICurrentUser _currentUser;

    public GetDashboardOverviewQueryHandler(
        IBaseRepository<Orchid> orchidRepository,
        IBaseRepository<AppDocument> documentRepository,
        IBaseRepository<Article> articleRepository,
        IBaseRepository<User> userRepository,
        IBaseRepository<UploadedImage> imageRepository,
        ICurrentUser currentUser)
    {
        _orchidRepository = orchidRepository;
        _documentRepository = documentRepository;
        _articleRepository = articleRepository;
        _userRepository = userRepository;
        _imageRepository = imageRepository;
        _currentUser = currentUser;
    }

    public async Task<DashboardOverviewDto> Handle(
        GetDashboardOverviewQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindByIdAsync(_currentUser.UserId);
        if (user == null)
        {
            throw new NotFoundException(nameof(User), _currentUser.UserId);
        }

        Expression<Func<Article, bool>>[] publishedCultivationFilter =
        [
            article => article.Type == ArticleCategoryType.CULTIVATION
                && article.IsPublished
        ];

        var totalOrchids = await _orchidRepository.CountAsync();
        var totalDocuments = await _documentRepository.CountAsync();
        var publishedCultivationArticles = await _articleRepository.CountAsync(publishedCultivationFilter);
        var totalUsers = await _userRepository.CountAsync();

        var recentOrchids = await _orchidRepository.FindAsync(
            orderBy: "CreatedAt desc",
            limit: RecentOrchidLimit);

        var imageIds = recentOrchids
            .SelectMany(orchid => orchid.UploadedImageIds)
            .Distinct()
            .ToList();
        var images = imageIds.Count == 0
            ? new List<UploadedImage>()
            : await _imageRepository.FindAsync(
                [image => imageIds.Contains(image.Id)],
                limit: int.MaxValue);
        var imagesById = images.ToDictionary(image => image.Id);

        return new DashboardOverviewDto
        {
            CurrentUser = new DashboardUserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                AvatarUrl = user.AvatarUrl
            },
            TotalOrchids = totalOrchids,
            TotalDocuments = totalDocuments,
            PublishedCultivationArticles = publishedCultivationArticles,
            TotalUsers = totalUsers,
            RecentOrchids = recentOrchids.Select(orchid => new RecentOrchidDto
            {
                Id = orchid.Id,
                Name = orchid.Name,
                EnglishName = orchid.EnglishName,
                Slug = orchid.Slug,
                IsPopular = orchid.IsPopular,
                CreatedAt = orchid.CreatedAt,
                ThumbnailImage = orchid.UploadedImageIds
                    .Where(imagesById.ContainsKey)
                    .Select(imageId => MapImage(imagesById[imageId]))
                    .FirstOrDefault()
            }).ToList()
        };
    }

    private static OrchidImageDto MapImage(UploadedImage image)
    {
        return new OrchidImageDto
        {
            Id = image.Id,
            Url = image.Url,
            PublicId = image.PublicId,
            FileName = image.FileName
        };
    }
}
