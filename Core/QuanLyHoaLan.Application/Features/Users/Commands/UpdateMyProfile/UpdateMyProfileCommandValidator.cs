using FluentValidation;

namespace QuanLyHoaLan.Application.Features.Users.Commands.UpdateMyProfile;

public class UpdateMyProfileCommandValidator : AbstractValidator<UpdateMyProfileCommand>
{
    public UpdateMyProfileCommandValidator()
    {
        RuleFor(command => command.FullName)
            .NotEmpty().WithMessage("Họ tên không được để trống.")
            .MaximumLength(256).WithMessage("Họ tên không được vượt quá 256 ký tự.");

        RuleFor(command => command.AvatarUrl)
            .MaximumLength(2048).WithMessage("URL ảnh đại diện không được vượt quá 2048 ký tự.")
            .Must(BeAValidHttpUrl).WithMessage("URL ảnh đại diện không hợp lệ.")
            .When(command => !string.IsNullOrWhiteSpace(command.AvatarUrl));
    }

    private static bool BeAValidHttpUrl(string? value)
    {
        return Uri.TryCreate(value, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}
