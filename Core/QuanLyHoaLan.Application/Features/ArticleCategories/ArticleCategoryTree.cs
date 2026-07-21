using QuanLyHoaLan.Application.DTOs.ArticleCategory;
using QuanLyHoaLan.Domain.Entities;

namespace QuanLyHoaLan.Application.Features.ArticleCategories;

internal static class ArticleCategoryTree
{
    public const int MaxDepth = 10;

    public static List<ArticleCategoryDto> Build(IEnumerable<ArticleCategory> categories, Guid? rootId = null)
    {
        var active = categories.Where(category => !category.IsDeleted).ToList();
        var byParent = active
            .GroupBy(category => category.ParentId ?? Guid.Empty)
            .ToDictionary(group => group.Key, group => group.OrderBy(category => category.Name).ToList());

        var parentNames = active.ToDictionary(category => category.Id, category => category.Name);
        return BuildChildren(rootId ?? Guid.Empty, byParent, parentNames, new HashSet<Guid>(), 0);
    }

    public static void EnsureValidParent(
        Guid currentId,
        Guid? parentId,
        IReadOnlyCollection<ArticleCategory> categories)
    {
        if (!parentId.HasValue)
        {
            return;
        }

        if (parentId.Value == currentId)
        {
            throw new InvalidOperationException("Danh mục không thể tự làm cha của chính mình.");
        }

        var byId = categories
            .Where(category => !category.IsDeleted)
            .ToDictionary(category => category.Id);

        if (!byId.ContainsKey(parentId.Value))
        {
            throw new InvalidOperationException("Danh mục cha không tồn tại hoặc đã bị xóa.");
        }

        var visited = new HashSet<Guid>();
        var cursor = parentId;
        var ancestorDepth = 0;
        while (cursor.HasValue)
        {
            if (cursor.Value == currentId)
            {
                throw new InvalidOperationException("Không thể tạo vòng lặp trong cây danh mục.");
            }

            if (!visited.Add(cursor.Value))
            {
                throw new InvalidOperationException("Cây danh mục hiện tại đang có vòng lặp.");
            }

            ancestorDepth++;
            cursor = byId.TryGetValue(cursor.Value, out var category) ? category.ParentId : null;
        }

        var subtreeDepth = byId.ContainsKey(currentId)
            ? GetSubtreeDepth(currentId, categories, new HashSet<Guid>())
            : 1;
        if (ancestorDepth + subtreeDepth > MaxDepth)
        {
            throw new InvalidOperationException($"Danh mục không được sâu quá {MaxDepth} cấp.");
        }
    }

    private static int GetSubtreeDepth(
        Guid categoryId,
        IReadOnlyCollection<ArticleCategory> categories,
        HashSet<Guid> ancestors)
    {
        if (!ancestors.Add(categoryId))
        {
            throw new InvalidOperationException("Cây danh mục hiện tại đang có vòng lặp.");
        }

        var children = categories
            .Where(category => !category.IsDeleted && category.ParentId == categoryId)
            .ToList();
        var depth = children.Count == 0
            ? 1
            : 1 + children.Max(child => GetSubtreeDepth(child.Id, categories, ancestors));

        ancestors.Remove(categoryId);
        return depth;
    }

    private static List<ArticleCategoryDto> BuildChildren(
        Guid parentId,
        IReadOnlyDictionary<Guid, List<ArticleCategory>> byParent,
        IReadOnlyDictionary<Guid, string> parentNames,
        HashSet<Guid> ancestors,
        int depth)
    {
        if (depth > MaxDepth || !byParent.TryGetValue(parentId, out var children))
        {
            return new List<ArticleCategoryDto>();
        }

        var result = new List<ArticleCategoryDto>();
        foreach (var category in children)
        {
            if (!ancestors.Add(category.Id))
            {
                continue;
            }

            var dto = Map(category, parentNames.GetValueOrDefault(category.ParentId ?? Guid.Empty) ?? string.Empty);
            dto.SubCategories = BuildChildren(category.Id, byParent, parentNames, ancestors, depth + 1);
            ancestors.Remove(category.Id);
            result.Add(dto);
        }

        return result;
    }

    public static ArticleCategoryDto Map(ArticleCategory category, string parentName = "")
    {
        return new ArticleCategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            Slug = category.Slug,
            Type = category.Type,
            ParentId = category.ParentId,
            ParentName = parentName
        };
    }
}
