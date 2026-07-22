using QuanLyHoaLan.Application.DTOs.Orchid;

namespace QuanLyHoaLan.Application.DTOs.Dashboard;

public class DashboardUserDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
}

public class RecentOrchidDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string EnglishName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public bool IsPopular { get; set; }
    public DateTime CreatedAt { get; set; }
    public OrchidImageDto? ThumbnailImage { get; set; }
}

public class DashboardOverviewDto
{
    public DashboardUserDto CurrentUser { get; set; } = new();
    public int TotalOrchids { get; set; }
    public int TotalDocuments { get; set; }
    public int PublishedCultivationArticles { get; set; }
    public int TotalUsers { get; set; }
    public List<RecentOrchidDto> RecentOrchids { get; set; } = new();
}
