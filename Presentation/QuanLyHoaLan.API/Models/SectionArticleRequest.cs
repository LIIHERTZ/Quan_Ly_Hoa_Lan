using QuanLyHoaLan.Application.Features.Articles.Commands.CreateArticle;
using QuanLyHoaLan.Application.Features.Articles.Commands.UpdateArticle;
using QuanLyHoaLan.Domain.Enums;

namespace QuanLyHoaLan.API.Models;

public class SectionArticleRequest
{
    public string? Title { get; set; }
    public string? Slug { get; set; }
    public string? Summary { get; set; }
    public string? Content { get; set; }
    public Guid? ThumbnailImageId { get; set; }
    public bool IsPublished { get; set; }
    public List<Guid>? ArticleCategoryIds { get; set; }
    public List<Guid>? OrchidIds { get; set; }
    public List<Guid>? DocumentIds { get; set; }

    public CreateArticleCommand ToCreateCommand(ArticleCategoryType type)
    {
        return new CreateArticleCommand
        {
            Title = Title!,
            Slug = Slug!,
            Summary = Summary!,
            Content = Content!,
            ThumbnailImageId = ThumbnailImageId,
            IsPublished = IsPublished,
            Type = type,
            ArticleCategoryIds = ArticleCategoryIds!,
            OrchidIds = OrchidIds!,
            DocumentIds = DocumentIds!
        };
    }

    public UpdateArticleCommand ToUpdateCommand(Guid id, ArticleCategoryType type)
    {
        var command = new UpdateArticleCommand
        {
            Id = id,
            Title = Title!,
            Slug = Slug!,
            Summary = Summary!,
            Content = Content!,
            ThumbnailImageId = ThumbnailImageId,
            IsPublished = IsPublished,
            Type = type,
            ArticleCategoryIds = ArticleCategoryIds!,
            OrchidIds = OrchidIds!,
            DocumentIds = DocumentIds!
        };
        command.RequireCurrentType(type);
        return command;
    }
}
