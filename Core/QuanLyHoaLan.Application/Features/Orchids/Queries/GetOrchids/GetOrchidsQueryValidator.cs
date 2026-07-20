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
    }
}
