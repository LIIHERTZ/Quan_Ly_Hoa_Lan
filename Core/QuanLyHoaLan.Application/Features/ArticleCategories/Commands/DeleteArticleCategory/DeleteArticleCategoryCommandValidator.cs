using FluentValidation;

namespace QuanLyHoaLan.Application.Features.ArticleCategories.Commands.DeleteArticleCategory;

public class DeleteArticleCategoryCommandValidator : AbstractValidator<DeleteArticleCategoryCommand>
{
    public DeleteArticleCategoryCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty().WithMessage("Id danh mục không hợp lệ.");
    }
}
