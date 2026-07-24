using MediatR;

namespace QuanLyHoaLan.Application.Features.Documents.Commands.UpdateDocument;

public class UpdateDocumentCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
}
