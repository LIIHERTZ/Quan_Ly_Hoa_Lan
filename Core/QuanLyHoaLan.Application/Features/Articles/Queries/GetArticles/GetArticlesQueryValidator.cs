using FluentValidation;

namespace QuanLyHoaLan.Application.Features.Articles.Queries.GetArticles;

public class GetArticlesQueryValidator : AbstractValidator<GetArticlesQuery>
{
    private static readonly string[] AllowedSortFields = ["title", "publishedat", "createdat"];

    public GetArticlesQueryValidator()
    {
        RuleFor(query => query.PageNumber)
            .InclusiveBetween(1, 1_000_000).WithMessage("Số trang phải từ 1 đến 1.000.000.");

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Kích thước trang phải từ 1 đến 100.");

        RuleFor(query => query.SearchTerm)
            .MaximumLength(256).WithMessage("Từ khóa không được vượt quá 256 ký tự.");

        RuleFor(query => query.OrchidId)
            .NotEqual(Guid.Empty).When(query => query.OrchidId.HasValue)
            .WithMessage("OrchidId không hợp lệ.");

        RuleFor(query => query.ArticleCategoryId)
            .NotEqual(Guid.Empty).When(query => query.ArticleCategoryId.HasValue)
            .WithMessage("ArticleCategoryId không hợp lệ.");

        RuleFor(query => query.SortBy)
            .Must(value => string.IsNullOrWhiteSpace(value)
                || AllowedSortFields.Contains(value.Trim().ToLowerInvariant()))
            .WithMessage("SortBy chỉ hỗ trợ: title, publishedAt, createdAt.");
    }
}
