using System;
using System.Collections.Generic;
using QuanLyHoaLan.Domain.Enums;

namespace QuanLyHoaLan.Application.DTOs.Article;

public class SimpleOrchidDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class SimpleArticleCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public ArticleCategoryType Type { get; set; }
}

public class ArticleDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    
    public Guid? ThumbnailImageId { get; set; }
    
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;

    public bool IsPublished { get; set; }
    public DateTimeOffset? PublishedAt { get; set; }

    public List<SimpleArticleCategoryDto> Categories { get; set; } = new();
    public List<Guid> OrchidIds { get; set; } = new();
    public List<Guid> DocumentIds { get; set; } = new();
}
