using MediatR;

namespace QuanLyHoaLan.Application.Features.DocumentCategories.Commands.UpdateDocumentCategory;

public class UpdateDocumentCategoryCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
}
