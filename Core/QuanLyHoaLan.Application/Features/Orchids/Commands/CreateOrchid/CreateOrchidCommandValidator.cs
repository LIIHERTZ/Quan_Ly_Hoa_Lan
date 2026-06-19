using FluentValidation;

namespace QuanLyHoaLan.Application.Features.Orchids.Commands.CreateOrchid;

public class CreateOrchidCommandValidator : AbstractValidator<CreateOrchidCommand>
{
    public CreateOrchidCommandValidator()
    {
        RuleFor(v => v.Name)
            .MaximumLength(256).WithMessage("Tên không được vượt quá 256 ký tự.")
            .NotEmpty().WithMessage("Tên không được để trống.");

        RuleFor(v => v.Slug)
            .MaximumLength(256).WithMessage("Slug không được vượt quá 256 ký tự.")
            .NotEmpty().WithMessage("Slug không được để trống.");

        RuleFor(v => v.CategoryIds)
            .NotEmpty().WithMessage("Danh mục không được để trống.");
    }
}
