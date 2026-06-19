using System;
using MediatR;

namespace QuanLyHoaLan.Application.Features.Articles.Commands.DeleteArticle;

public class DeleteArticleCommand : IRequest<Unit>
{
    public Guid Id { get; set; }

    public DeleteArticleCommand(Guid id)
    {
        Id = id;
    }
}
