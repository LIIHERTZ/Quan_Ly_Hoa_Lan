using MediatR;
using QuanLyHoaLan.Application.DTOs.DocumentCategory;

namespace QuanLyHoaLan.Application.Features.DocumentCategories.Queries.GetDocumentCategoryTree;

public record GetDocumentCategoryTreeQuery : IRequest<List<DocumentCategoryDto>>;
