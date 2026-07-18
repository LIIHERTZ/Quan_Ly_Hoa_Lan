using FluentValidation;

namespace QuanLyHoaLan.Application.Features.Articles.Commands.UpdateArticle;

public class UpdateArticleCommandValidator : AbstractValidator<UpdateArticleCommand>
{
    public UpdateArticleCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Id không được để trống.");

        RuleFor(v => v.Title)
            .NotEmpty().WithMessage("Tiêu đề không được để trống.")
            .MaximumLength(200).WithMessage("Tiêu đề không được vượt quá 200 ký tự.");

        RuleFor(v => v.Slug)
            .NotEmpty().WithMessage("Slug không được để trống.")
            .MaximumLength(200).WithMessage("Slug không được vượt quá 200 ký tự.");
            
        RuleFor(v => v.Summary)
            .NotEmpty().WithMessage("Tóm tắt không được để trống.");

        RuleFor(v => v.Content)
            .NotEmpty().WithMessage("Nội dung không được để trống.");
    }
}
