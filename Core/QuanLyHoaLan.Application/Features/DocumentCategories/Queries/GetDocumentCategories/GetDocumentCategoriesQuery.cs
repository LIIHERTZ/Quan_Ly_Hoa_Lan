using MediatR;
using QuanLyHoaLan.Application.Common.Models;
using QuanLyHoaLan.Application.DTOs.DocumentCategory;

namespace QuanLyHoaLan.Application.Features.DocumentCategories.Queries.GetDocumentCategories;

public class GetDocumentCategoriesQuery
    : PagedRequest, IRequest<PaginatedList<DocumentCategoryDto>>
{
    public Guid? ParentId { get; set; }
}
