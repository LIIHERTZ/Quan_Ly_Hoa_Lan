using MediatR;
using QuanLyHoaLan.Application.DTOs.DocumentCategory;

namespace QuanLyHoaLan.Application.Features.DocumentCategories.Queries.GetDocumentCategoryById;

public record GetDocumentCategoryByIdQuery(Guid Id) : IRequest<DocumentCategoryDto>;
