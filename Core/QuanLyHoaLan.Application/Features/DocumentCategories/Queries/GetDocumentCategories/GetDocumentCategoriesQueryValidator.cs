using FluentValidation;

namespace QuanLyHoaLan.Application.Features.DocumentCategories.Queries.GetDocumentCategories;

public class GetDocumentCategoriesQueryValidator : AbstractValidator<GetDocumentCategoriesQuery>
{
    public GetDocumentCategoriesQueryValidator()
    {
        RuleFor(query => query.PageNumber)
            .GreaterThan(0).WithMessage("PageNumber phải lớn hơn 0.");

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100).WithMessage("PageSize phải từ 1 đến 100.");

        RuleFor(query => query.SearchTerm)
            .MaximumLength(256).WithMessage("Từ khóa tìm kiếm không được vượt quá 256 ký tự.");

        RuleFor(query => query.ParentId)
            .NotEqual(Guid.Empty).When(query => query.ParentId.HasValue)
            .WithMessage("ParentId không hợp lệ.");
    }
}
