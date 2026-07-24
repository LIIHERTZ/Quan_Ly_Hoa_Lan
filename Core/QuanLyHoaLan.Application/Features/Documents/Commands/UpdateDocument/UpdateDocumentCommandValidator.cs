using FluentValidation;

namespace QuanLyHoaLan.Application.Features.Documents.Commands.UpdateDocument;

public class UpdateDocumentCommandValidator : AbstractValidator<UpdateDocumentCommand>
{
    public UpdateDocumentCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty().WithMessage("Id tài liệu là bắt buộc.");

        RuleFor(command => command.Title)
            .NotEmpty().WithMessage("Tiêu đề tài liệu là bắt buộc.")
            .MaximumLength(255).WithMessage("Tiêu đề không được vượt quá 255 ký tự.");

        RuleFor(command => command.Description)
            .MaximumLength(1000).WithMessage("Mô tả không được vượt quá 1000 ký tự.");

        RuleFor(command => command.CategoryId)
            .NotEmpty().WithMessage("Danh mục tài liệu là bắt buộc.");
    }
}
