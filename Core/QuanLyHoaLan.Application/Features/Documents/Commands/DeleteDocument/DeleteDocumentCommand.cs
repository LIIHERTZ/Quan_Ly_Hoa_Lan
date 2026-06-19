using MediatR;

namespace QuanLyHoaLan.Application.Features.Documents.Commands.DeleteDocument;

public class DeleteDocumentCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}
