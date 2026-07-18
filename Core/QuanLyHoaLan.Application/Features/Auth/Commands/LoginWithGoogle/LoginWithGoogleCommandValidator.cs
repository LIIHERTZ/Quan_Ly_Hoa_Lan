using FluentValidation;

namespace QuanLyHoaLan.Application.Features.Auth.Commands.LoginWithGoogle;

public class LoginWithGoogleCommandValidator : AbstractValidator<LoginWithGoogleCommand>
{
    public LoginWithGoogleCommandValidator()
    {
        RuleFor(v => v.IdToken)
            .NotEmpty().WithMessage("IdToken không được để trống.");
    }
}
