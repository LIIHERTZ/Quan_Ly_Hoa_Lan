using FluentValidation;

namespace QuanLyHoaLan.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Id không được để trống.");

        RuleFor(v => v.Name)
            .MaximumLength(256).WithMessage("Tên không được vượt quá 256 ký tự.")
            .NotEmpty().WithMessage("Tên không được để trống.");

        RuleFor(v => v.Slug)
            .MaximumLength(256).WithMessage("Slug không được vượt quá 256 ký tự.")
            .NotEmpty().WithMessage("Slug không được để trống.");
    }
}
