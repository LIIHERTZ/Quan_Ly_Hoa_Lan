using MediatR;

namespace QuanLyHoaLan.Application.Features.DocumentCategories.Commands.CreateDocumentCategory;

public class CreateDocumentCategoryCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
}
