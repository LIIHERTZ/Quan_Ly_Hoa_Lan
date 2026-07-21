using MediatR;
using System.Text.Json.Serialization;
using QuanLyHoaLan.Domain.Enums;

namespace QuanLyHoaLan.Application.Features.ArticleCategories.Commands.UpdateArticleCategory;

public class UpdateArticleCategoryCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }

    [JsonIgnore]
    public ArticleCategoryType Type { get; set; }
}
