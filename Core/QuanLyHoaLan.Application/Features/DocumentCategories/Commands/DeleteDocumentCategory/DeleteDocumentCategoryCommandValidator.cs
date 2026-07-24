using FluentValidation;

namespace QuanLyHoaLan.Application.Features.DocumentCategories.Commands.DeleteDocumentCategory;

public class DeleteDocumentCategoryCommandValidator : AbstractValidator<DeleteDocumentCategoryCommand>
{
    public DeleteDocumentCategoryCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty()
            .WithMessage("Id danh mục là bắt buộc.");
    }
}
