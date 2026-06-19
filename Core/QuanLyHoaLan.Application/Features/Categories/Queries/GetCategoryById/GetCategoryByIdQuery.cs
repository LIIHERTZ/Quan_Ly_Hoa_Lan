using System;
using MediatR;
using QuanLyHoaLan.Application.DTOs.Category;

namespace QuanLyHoaLan.Application.Features.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQuery : IRequest<CategoryDto>
{
    public Guid Id { get; set; }

    public GetCategoryByIdQuery(Guid id)
    {
        Id = id;
    }
}
