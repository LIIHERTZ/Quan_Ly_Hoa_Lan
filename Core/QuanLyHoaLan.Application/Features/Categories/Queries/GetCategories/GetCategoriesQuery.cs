using MediatR;
using QuanLyHoaLan.Application.Common.Models;
using QuanLyHoaLan.Application.DTOs.Category;

namespace QuanLyHoaLan.Application.Features.Categories.Queries.GetCategories;

public class GetCategoriesQuery : PagedRequest, IRequest<PaginatedList<CategoryDto>>
{
}
