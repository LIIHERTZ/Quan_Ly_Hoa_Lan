using FluentValidation;

namespace QuanLyHoaLan.Application.Features.Documents.Commands.UploadDocument;

public class UploadDocumentCommandValidator : AbstractValidator<UploadDocumentCommand>
{
    public UploadDocumentCommandValidator()
    {
        RuleFor(command => command.FileStream)
            .NotNull()
            .WithMessage("File stream is required.");

        RuleFor(command => command.FileName)
            .NotEmpty()
            .WithMessage("File name is required.");

        RuleFor(command => command.CategoryId)
            .NotEmpty()
            .WithMessage("Danh mục tài liệu là bắt buộc.");

        RuleFor(command => command.Title)
            .NotEmpty()
            .WithMessage("Tiêu đề tài liệu không được để trống.")
            .MaximumLength(255)
            .WithMessage("Tiêu đề không được vượt quá 255 ký tự.");

        RuleFor(command => command.Description)
            .MaximumLength(1000)
            .WithMessage("Mô tả không được vượt quá 1000 ký tự.");
    }
}
