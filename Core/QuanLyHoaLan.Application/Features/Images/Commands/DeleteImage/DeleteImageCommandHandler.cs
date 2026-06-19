using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QuanLyHoaLan.Application.Interfaces.Services;

namespace QuanLyHoaLan.Application.Features.Images.Commands.DeleteImage;

public class DeleteImageCommandHandler : IRequestHandler<DeleteImageCommand, bool>
{
    private readonly IImageService _imageService;

    public DeleteImageCommandHandler(IImageService imageService)
    {
        _imageService = imageService;
    }

    public async Task<bool> Handle(DeleteImageCommand request, CancellationToken cancellationToken)
    {
        var result = await _imageService.DeleteImageAsync(request.PublicId);

        if (!result)
        {
            throw new Exception("Failed to delete image.");
        }

        return result;
    }
}
