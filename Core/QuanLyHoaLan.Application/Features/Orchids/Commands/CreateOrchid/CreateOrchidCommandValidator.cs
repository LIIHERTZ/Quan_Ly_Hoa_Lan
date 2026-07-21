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

        AddStringListRules(command => command.Colors, "Màu sắc");
        AddStringListRules(command => command.Regions, "Vùng phân bố");
        AddStringListRules(command => command.BloomSeasons, "Mùa ra hoa");
    }

    private void AddStringListRules(
        System.Linq.Expressions.Expression<Func<CreateOrchidCommand, IEnumerable<string>>> selector,
        string fieldName)
    {
        RuleFor(selector)
            .NotNull().WithMessage($"{fieldName} không được null.")
            .Must(values => values == null || values.Count() <= 50)
            .WithMessage($"{fieldName} không được vượt quá 50 giá trị.");

        RuleForEach(selector)
            .NotEmpty().WithMessage($"Giá trị {fieldName.ToLowerInvariant()} không được để trống.")
            .MaximumLength(200).WithMessage($"Mỗi giá trị {fieldName.ToLowerInvariant()} không được vượt quá 200 ký tự.");
    }
}
