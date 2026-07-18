using FluentValidation;
using System.IO;

namespace QuanLyHoaLan.Application.Features.Images.Commands.UploadImage;

public class UploadImageCommandValidator : AbstractValidator<UploadImageCommand>
{
    public UploadImageCommandValidator()
    {
        RuleFor(v => v.FileName)
            .NotEmpty().WithMessage("Tên file không được để trống.");
            
        RuleFor(v => v.FileStream)
            .NotNull().WithMessage("File không được để trống.")
            .Must(stream => stream != Stream.Null && stream.Length > 0).WithMessage("File không hợp lệ hoặc trống.");
    }
}
