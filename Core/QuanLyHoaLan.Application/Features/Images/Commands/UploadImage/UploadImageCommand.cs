using System.IO;
using MediatR;
using QuanLyHoaLan.Application.Models;

namespace QuanLyHoaLan.Application.Features.Images.Commands.UploadImage;

public class UploadImageCommand : IRequest<ImageUploadResult>
{
    public Stream FileStream { get; set; } = Stream.Null;
    public string FileName { get; set; } = string.Empty;
}
