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
            .NotEmpty().WithMessage("Slug is required.")
            .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .WithMessage("Slug chỉ được chứa chữ thường không dấu, số và dấu gạch ngang.");

        RuleFor(v => v.Content)
            .NotEmpty().WithMessage("Content is required.");

        RuleFor(v => v.Summary)
            .MaximumLength(500).WithMessage("Tóm tắt không được vượt quá 500 ký tự.");

        RuleFor(v => v.ArticleCategoryIds)
            .NotNull().WithMessage("Danh mục bài viết không được null.")
            .NotEmpty().WithMessage("Bài viết phải thuộc ít nhất một danh mục.")
            .Must(ids => ids == null || ids.Count <= 20).WithMessage("Một bài viết chỉ được thuộc tối đa 20 danh mục.")
            .Must(ids => ids == null || ids.All(id => id != Guid.Empty)).WithMessage("Id danh mục không hợp lệ.")
            .Must(ids => ids == null || ids.Distinct().Count() == ids.Count).WithMessage("Danh mục bài viết không được trùng.");

        AddIdListRules(v => v.OrchidIds, "Orchid", 50);
        AddIdListRules(v => v.DocumentIds, "tài liệu", 20);
    }

    private void AddIdListRules(
        System.Linq.Expressions.Expression<Func<CreateArticleCommand, List<Guid>>> selector,
        string fieldName,
        int maximum)
    {
        RuleFor(selector)
            .NotNull().WithMessage($"Danh sách {fieldName} không được null.")
            .Must(ids => ids == null || ids.Count <= maximum).WithMessage($"Danh sách {fieldName} vượt quá {maximum} phần tử.")
            .Must(ids => ids == null || ids.All(id => id != Guid.Empty)).WithMessage($"Danh sách {fieldName} chứa Id không hợp lệ.")
            .Must(ids => ids == null || ids.Distinct().Count() == ids.Count).WithMessage($"Danh sách {fieldName} không được chứa Id trùng.");
    }
}
