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

        AddStringListRules(query => query.Colors, "Màu sắc");
        AddStringListRules(query => query.Regions, "Vùng phân bố");
        AddStringListRules(query => query.BloomSeasons, "Mùa ra hoa");
    }

    private void AddStringListRules(
        System.Linq.Expressions.Expression<Func<GetOrchidsQuery, IEnumerable<string>>> selector,
        string fieldName)
    {
        RuleFor(selector)
            .NotNull().WithMessage($"{fieldName} không được null.")
            .Must(values => values == null || values.Count() <= 50)
            .WithMessage($"{fieldName} không được vượt quá 50 giá trị.");

        RuleForEach(selector)
            .NotEmpty().WithMessage($"Giá trị {fieldName.ToLowerInvariant()} không được để trống.")
            .MaximumLength(200).WithMessage($"Mỗi giá trị {fieldName.ToLowerInvariant()} không được vượt quá 200 ký tự.");
    }
}
