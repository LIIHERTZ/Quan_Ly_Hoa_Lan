using FluentValidation;

namespace QuanLyHoaLan.Application.Features.Orchids.Commands.CreateOrchid;

public class CreateOrchidCommandValidator : AbstractValidator<CreateOrchidCommand>
{
    public CreateOrchidCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("Tên không được để trống.")
            .MaximumLength(256).WithMessage("Tên không được vượt quá 256 ký tự.");

        RuleFor(command => command.Slug)
            .NotEmpty().WithMessage("Slug không được để trống.")
            .MaximumLength(256).WithMessage("Slug không được vượt quá 256 ký tự.");

        RuleFor(command => command.CategoryIds)
            .NotEmpty().WithMessage("Danh mục không được để trống.");

        RuleFor(command => command.Colors)
            .NotNull().WithMessage("Màu sắc không được null.")
            .Must(values => values == null || values.Count <= 50)
            .WithMessage("Màu sắc không được vượt quá 50 giá trị.");
        RuleForEach(command => command.Colors).IsInEnum();

        RuleFor(command => command.Regions)
            .NotNull().WithMessage("Vùng phân bố không được null.")
            .Must(values => values == null || values.Count <= 50)
            .WithMessage("Vùng phân bố không được vượt quá 50 giá trị.");
        RuleForEach(command => command.Regions).IsInEnum();

        RuleFor(command => command.BloomSeasons)
            .NotNull().WithMessage("Mùa ra hoa không được null.")
            .Must(values => values == null || values.Count <= 50)
            .WithMessage("Mùa ra hoa không được vượt quá 50 giá trị.");
        RuleForEach(command => command.BloomSeasons).IsInEnum();
    }
}
