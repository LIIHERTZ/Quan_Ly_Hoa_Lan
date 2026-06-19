using FluentValidation;

namespace QuanLyHoaLan.Application.Features.Articles.Commands.CreateArticle;

public class CreateArticleCommandValidator : AbstractValidator<CreateArticleCommand>
{
    public CreateArticleCommandValidator()
    {
        RuleFor(v => v.Title)
            .MaximumLength(255).WithMessage("Title must not exceed 255 characters.")
            .NotEmpty().WithMessage("Title is required.");

        RuleFor(v => v.Slug)
            .MaximumLength(255).WithMessage("Slug must not exceed 255 characters.")
            .NotEmpty().WithMessage("Slug is required.");

        RuleFor(v => v.Content)
            .NotEmpty().WithMessage("Content is required.");
    }
}
