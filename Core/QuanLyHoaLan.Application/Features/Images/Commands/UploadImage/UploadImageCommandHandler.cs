using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QuanLyHoaLan.Application.Interfaces.Services;
using QuanLyHoaLan.Application.Models;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Images.Commands.UploadImage;

public class UploadImageCommandHandler : IRequestHandler<UploadImageCommand, ImageUploadResult>
{
    private readonly IImageService _imageService;
    private readonly IBaseRepository<UploadedImage> _imageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UploadImageCommandHandler(
        IImageService imageService, 
        IBaseRepository<UploadedImage> imageRepository,
        IUnitOfWork unitOfWork)
    {
        _imageService = imageService;
        _imageRepository = imageRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ImageUploadResult> Handle(UploadImageCommand request, CancellationToken cancellationToken)
    {
        var result = await _imageService.UploadImageAsync(request.FileStream, request.FileName);

        if (result == null)
        {
            throw new Exception("Image upload failed.");
        }

        var uploadedImage = new UploadedImage
        {
            Url = result.Url,
            PublicId = result.PublicId,
            FileName = request.FileName
        };

        await _imageRepository.InsertAsync(uploadedImage, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return result;
    }
}
