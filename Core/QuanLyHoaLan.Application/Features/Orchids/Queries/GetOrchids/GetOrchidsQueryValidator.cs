using FluentValidation;

namespace QuanLyHoaLan.Application.Features.Orchids.Queries.GetOrchids;

public class GetOrchidsQueryValidator : AbstractValidator<GetOrchidsQuery>
{
    private static readonly string[] AllowedSortFields =
    [
        "name",
        "englishname",
        "displayorder",
        "createdat",
        "ispopular"
    ];

    public GetOrchidsQueryValidator()
    {
        RuleFor(query => query.PageNumber)
            .InclusiveBetween(1, 1_000_000).WithMessage("Số trang phải từ 1 đến 1.000.000.");

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Kích thước trang phải từ 1 đến 100.");

        RuleFor(query => query.SearchTerm)
            .MaximumLength(256).WithMessage("Từ khóa tìm kiếm không được vượt quá 256 ký tự.");

        RuleFor(query => query.SortBy)
            .Must(sortBy => string.IsNullOrWhiteSpace(sortBy)
                || AllowedSortFields.Contains(sortBy.Trim().ToLowerInvariant()))
            .WithMessage("SortBy chỉ hỗ trợ: name, englishName, displayOrder, createdAt, isPopular.");

        RuleFor(query => query.Colors)
            .NotNull().WithMessage("Màu sắc không được null.")
            .Must(values => values == null || values.Count <= 50)
            .WithMessage("Màu sắc không được vượt quá 50 giá trị.");
        RuleForEach(query => query.Colors).IsInEnum();

        RuleFor(query => query.Regions)
            .NotNull().WithMessage("Vùng phân bố không được null.")
            .Must(values => values == null || values.Count <= 50)
            .WithMessage("Vùng phân bố không được vượt quá 50 giá trị.");
        RuleForEach(query => query.Regions).IsInEnum();

        RuleFor(query => query.BloomSeasons)
            .NotNull().WithMessage("Mùa ra hoa không được null.")
            .Must(values => values == null || values.Count <= 50)
            .WithMessage("Mùa ra hoa không được vượt quá 50 giá trị.");
        RuleForEach(query => query.BloomSeasons).IsInEnum();
    }
}
