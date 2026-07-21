using FluentValidation;

namespace QuanLyHoaLan.Application.Features.ArticleCategories.Commands.CreateArticleCategory;

public class CreateArticleCategoryCommandValidator : AbstractValidator<CreateArticleCategoryCommand>
{
    public CreateArticleCategoryCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("Tên danh mục không được để trống.")
            .MaximumLength(256).WithMessage("Tên danh mục không được vượt quá 256 ký tự.");

        RuleFor(command => command.Description)
            .MaximumLength(1000).WithMessage("Mô tả không được vượt quá 1000 ký tự.");

        RuleFor(command => command.Slug)
            .NotEmpty().WithMessage("Slug không được để trống.")
            .MaximumLength(256).WithMessage("Slug không được vượt quá 256 ký tự.")
            .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .WithMessage("Slug chỉ được chứa chữ thường không dấu, số và dấu gạch ngang.");
    }
}
