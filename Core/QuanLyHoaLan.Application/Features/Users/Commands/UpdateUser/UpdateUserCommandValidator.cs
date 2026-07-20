using FluentValidation;

namespace QuanLyHoaLan.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id người dùng không hợp lệ.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email không được để trống.")
            .EmailAddress().WithMessage("Email không hợp lệ.")
            .MaximumLength(256).WithMessage("Email không được vượt quá 256 ký tự.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Họ tên không được để trống.")
            .MaximumLength(256).WithMessage("Họ tên không được vượt quá 256 ký tự.");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("RoleId không được để trống.");

        RuleFor(x => x.AvatarUrl)
            .MaximumLength(2048).WithMessage("URL ảnh đại diện không được vượt quá 2048 ký tự.");
    }
}
