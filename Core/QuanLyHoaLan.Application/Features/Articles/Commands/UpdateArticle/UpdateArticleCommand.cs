using System;
using System.Collections.Generic;
using MediatR;
using QuanLyHoaLan.Domain.Enums;

namespace QuanLyHoaLan.Application.Features.Articles.Commands.UpdateArticle;

public class UpdateArticleCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid? ThumbnailImageId { get; set; }
    public bool IsPublished { get; set; }
    public ArticleCategoryType? Type { get; set; }
    public List<Guid> ArticleCategoryIds { get; set; } = new();
    public List<Guid> OrchidIds { get; set; } = new();
    public List<Guid> DocumentIds { get; set; } = new();

    internal ArticleCategoryType? ExpectedType { get; private set; }

    public void RequireCurrentType(ArticleCategoryType type)
    {
        ExpectedType = type;
    }
}
