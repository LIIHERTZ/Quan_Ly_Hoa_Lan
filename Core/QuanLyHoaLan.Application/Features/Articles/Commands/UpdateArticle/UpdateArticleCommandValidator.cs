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
            .MaximumLength(255).WithMessage("Slug không được vượt quá 255 ký tự.")
            .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .WithMessage("Slug chỉ được chứa chữ thường không dấu, số và dấu gạch ngang.");
            
        RuleFor(v => v.Summary)
            .MaximumLength(500).WithMessage("Tóm tắt không được vượt quá 500 ký tự.");

        RuleFor(v => v.Content)
            .NotEmpty().WithMessage("Nội dung không được để trống.");

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
        System.Linq.Expressions.Expression<Func<UpdateArticleCommand, List<Guid>>> selector,
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
