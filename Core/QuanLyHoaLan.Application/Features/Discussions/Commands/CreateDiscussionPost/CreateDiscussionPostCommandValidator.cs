using FluentValidation;

namespace QuanLyHoaLan.Application.Features.Discussions.Commands.CreateDiscussionPost;

public class CreateDiscussionPostCommandValidator : AbstractValidator<CreateDiscussionPostCommand>
{
    public CreateDiscussionPostCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Tiêu đề không được để trống.")
            .MaximumLength(255).WithMessage("Tiêu đề không được vượt quá 255 ký tự.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Nội dung không được để trống.");
    }
}
