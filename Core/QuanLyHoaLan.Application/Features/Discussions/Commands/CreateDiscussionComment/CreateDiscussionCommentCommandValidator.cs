using FluentValidation;

namespace QuanLyHoaLan.Application.Features.Discussions.Commands.CreateDiscussionComment;

public class CreateDiscussionCommentCommandValidator : AbstractValidator<CreateDiscussionCommentCommand>
{
    public CreateDiscussionCommentCommandValidator()
    {
        RuleFor(x => x.PostId)
            .NotEmpty().WithMessage("ID bài viết không hợp lệ.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Nội dung bình luận không được để trống.")
            .MaximumLength(2000).WithMessage("Nội dung không được vượt quá 2000 ký tự.");
    }
}
