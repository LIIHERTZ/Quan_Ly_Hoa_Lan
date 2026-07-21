using System.Linq.Expressions;
using QuanLyHoaLan.Domain.Common;
using QuanLyHoaLan.Domain.Entities;
using QuanLyHoaLan.Domain.Interfaces.Repositories;

namespace QuanLyHoaLan.Application.Features.Articles;

internal static class ArticleRelationValidator
{
    public static List<Guid> NormalizeIds(IEnumerable<Guid>? ids)
    {
        return ids?.Where(id => id != Guid.Empty).Distinct().ToList() ?? new List<Guid>();
    }

    public static async Task<List<ArticleCategory>> GetLeafCategoriesAsync(
        IEnumerable<Guid>? ids,
        IBaseRepository<ArticleCategory> categoryRepository)
    {
        var categoryIds = NormalizeIds(ids);
        if (categoryIds.Count == 0)
        {
            throw new InvalidOperationException("Bài viết phải thuộc ít nhất một danh mục bài viết.");
        }

        Expression<Func<ArticleCategory, bool>>[] filters =
            [category => categoryIds.Contains(category.Id)];
        var categories = await categoryRepository.FindAsync(filters, limit: categoryIds.Count);
        if (categories.Count != categoryIds.Count)
        {
            throw new InvalidOperationException("Có danh mục bài viết không tồn tại hoặc đã bị xóa.");
        }

        Expression<Func<ArticleCategory, bool>>[] childFilters =
            [category => category.ParentId.HasValue && categoryIds.Contains(category.ParentId.Value)];
        if (await categoryRepository.AnyAsync(childFilters))
        {
            throw new InvalidOperationException("Chỉ được gắn bài viết vào danh mục lá (danh mục không có danh mục con).");
        }

        return categories;
    }

    public static async Task<List<Guid>> EnsureIdsExistAsync<TEntity>(
        IEnumerable<Guid>? ids,
        IBaseRepository<TEntity> repository,
        string fieldName)
        where TEntity : BaseEntity
    {
        var normalized = NormalizeIds(ids);
        if (normalized.Count > 0 && !await repository.ExistsAsync(normalized))
        {
            throw new InvalidOperationException($"Có {fieldName} không tồn tại hoặc đã bị xóa.");
        }

        return normalized;
    }

    public static async Task EnsureOptionalIdExistsAsync<TEntity>(
        Guid? id,
        IBaseRepository<TEntity> repository,
        string fieldName)
        where TEntity : BaseEntity
    {
        if (id.HasValue && !await repository.ExistsAsync(id.Value))
        {
            throw new InvalidOperationException($"{fieldName} không tồn tại hoặc đã bị xóa.");
        }
    }
}
