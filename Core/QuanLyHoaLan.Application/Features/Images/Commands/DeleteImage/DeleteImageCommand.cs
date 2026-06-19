using MediatR;

namespace QuanLyHoaLan.Application.Features.Images.Commands.DeleteImage;

public class DeleteImageCommand : IRequest<bool>
{
    public string PublicId { get; set; } = string.Empty;

    public DeleteImageCommand(string publicId)
    {
        PublicId = publicId;
    }
}
