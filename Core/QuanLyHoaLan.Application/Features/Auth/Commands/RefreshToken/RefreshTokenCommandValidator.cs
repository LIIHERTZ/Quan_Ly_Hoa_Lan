using FluentValidation;

namespace QuanLyHoaLan.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(v => v.Token)
            .NotEmpty().WithMessage("Token không được để trống.");

        RuleFor(v => v.RefreshToken)
            .NotEmpty().WithMessage("RefreshToken không được để trống.");
    }
}
