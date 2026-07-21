using FluentValidation;

namespace QuanLyHoaLan.Application.Features.ArticleCategories.Queries.GetArticleCategories;

public class GetArticleCategoriesQueryValidator : AbstractValidator<GetArticleCategoriesQuery>
{
    private static readonly string[] AllowedSortFields = ["name", "slug", "createdat"];

    public GetArticleCategoriesQueryValidator()
    {
        RuleFor(query => query.PageNumber)
            .InclusiveBetween(1, 1_000_000).WithMessage("Số trang phải từ 1 đến 1.000.000.");

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Kích thước trang phải từ 1 đến 100.");

        RuleFor(query => query.SearchTerm)
            .MaximumLength(256).WithMessage("Từ khóa không được vượt quá 256 ký tự.");

        RuleFor(query => query.SortBy)
            .Must(value => string.IsNullOrWhiteSpace(value)
                || AllowedSortFields.Contains(value.Trim().ToLowerInvariant()))
            .WithMessage("SortBy chỉ hỗ trợ: name, slug, createdAt.");
    }
}
