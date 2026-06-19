using MediatR;
using QuanLyHoaLan.Application.Common.Models;
using QuanLyHoaLan.Application.DTOs.Article;
using System;

namespace QuanLyHoaLan.Application.Features.Articles.Queries.GetArticles;

public class GetArticlesQuery : PagedRequest, IRequest<PaginatedList<ArticleDto>>
{
    public Guid? OrchidId { get; set; }
    public bool? IsPublished { get; set; }
}
