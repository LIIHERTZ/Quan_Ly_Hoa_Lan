using System;
using MediatR;
using QuanLyHoaLan.Domain.Enums;

namespace QuanLyHoaLan.Application.Features.Articles.Commands.DeleteArticle;

public class DeleteArticleCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public ArticleCategoryType? Type { get; set; }

    public DeleteArticleCommand(Guid id, ArticleCategoryType? type = null)
    {
        Id = id;
        Type = type;
    }
}
