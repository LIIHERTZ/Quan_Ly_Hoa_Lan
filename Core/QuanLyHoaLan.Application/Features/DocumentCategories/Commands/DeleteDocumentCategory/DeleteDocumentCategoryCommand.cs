using MediatR;

namespace QuanLyHoaLan.Application.Features.DocumentCategories.Commands.DeleteDocumentCategory;

public record DeleteDocumentCategoryCommand(Guid Id) : IRequest<bool>;
