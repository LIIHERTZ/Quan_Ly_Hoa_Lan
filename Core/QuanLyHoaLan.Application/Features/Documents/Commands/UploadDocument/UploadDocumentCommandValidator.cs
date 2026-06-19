using FluentValidation;

namespace QuanLyHoaLan.Application.Features.Documents.Commands.UploadDocument;

public class UploadDocumentCommandValidator : AbstractValidator<UploadDocumentCommand>
{
    public UploadDocumentCommandValidator()
    {
        RuleFor(x => x.FileStream)
            .NotNull()
            .WithMessage("File stream is required.");
        
        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage("File name is required.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Tiêu đề tài liệu không được để trống.")
            .MaximumLength(255)
            .WithMessage("Tiêu đề không được vượt quá 255 ký tự.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Mô tả không được vượt quá 1000 ký tự.");
    }
}
