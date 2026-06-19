using System;
using MediatR;
using QuanLyHoaLan.Application.DTOs.Article;

namespace QuanLyHoaLan.Application.Features.Articles.Queries.GetArticleById;

public class GetArticleByIdQuery : IRequest<ArticleDto>
{
    public Guid Id { get; set; }

    public GetArticleByIdQuery(Guid id)
    {
        Id = id;
    }
}
