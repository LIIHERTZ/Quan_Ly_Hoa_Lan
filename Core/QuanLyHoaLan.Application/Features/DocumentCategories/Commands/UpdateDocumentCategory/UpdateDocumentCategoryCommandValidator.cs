using FluentValidation;

namespace QuanLyHoaLan.Application.Features.DocumentCategories.Commands.UpdateDocumentCategory;

public class UpdateDocumentCategoryCommandValidator : AbstractValidator<UpdateDocumentCategoryCommand>
{
    public UpdateDocumentCategoryCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty().WithMessage("Id danh mục là bắt buộc.");

        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("Tên danh mục là bắt buộc.")
            .MaximumLength(256).WithMessage("Tên danh mục không được vượt quá 256 ký tự.");

        RuleFor(command => command.Slug)
            .NotEmpty().WithMessage("Slug là bắt buộc.")
            .MaximumLength(256).WithMessage("Slug không được vượt quá 256 ký tự.")
            .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .WithMessage("Slug chỉ gồm chữ thường không dấu, số và dấu gạch ngang.");

        RuleFor(command => command.Description)
            .MaximumLength(1000).WithMessage("Mô tả không được vượt quá 1000 ký tự.");

        RuleFor(command => command.ParentId)
            .NotEqual(Guid.Empty).When(command => command.ParentId.HasValue)
            .WithMessage("ParentId không hợp lệ.");
    }
}
