using QuanLyHoaLan.Application.DTOs.DocumentCategory;
using QuanLyHoaLan.Domain.Entities;

namespace QuanLyHoaLan.Application.Features.DocumentCategories;

internal static class DocumentCategoryTree
{
    public const int MaxDepth = 10;

    public static List<DocumentCategoryDto> Build(
        IEnumerable<DocumentCategory> categories,
        IReadOnlyDictionary<Guid, int>? documentCounts = null,
        Guid? rootId = null)
    {
        var active = categories.Where(category => !category.IsDeleted).ToList();
        var byParent = active
            .GroupBy(category => category.ParentId ?? Guid.Empty)
            .ToDictionary(
                group => group.Key,
                group => group.OrderBy(category => category.Name).ToList());
        var parentNames = active.ToDictionary(category => category.Id, category => category.Name);

        return BuildChildren(
            rootId ?? Guid.Empty,
            byParent,
            parentNames,
            documentCounts ?? new Dictionary<Guid, int>(),
            new HashSet<Guid>(),
            0);
    }

    public static DocumentCategoryDto Map(
        DocumentCategory category,
        string parentName = "",
        int documentCount = 0)
    {
        return new DocumentCategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            Slug = category.Slug,
            ParentId = category.ParentId,
            ParentName = parentName,
            DocumentCount = documentCount
        };
    }

    public static void EnsureValidParent(
        Guid currentId,
        Guid? parentId,
        IReadOnlyCollection<DocumentCategory> categories)
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
            cursor = byId.TryGetValue(cursor.Value, out var category)
                ? category.ParentId
                : null;
        }

        var subtreeDepth = byId.ContainsKey(currentId)
            ? GetSubtreeDepth(currentId, categories, new HashSet<Guid>())
            : 1;
        if (ancestorDepth + subtreeDepth > MaxDepth)
        {
            throw new InvalidOperationException(
                $"Danh mục tài liệu không được sâu quá {MaxDepth} cấp.");
        }
    }

    public static HashSet<Guid> ExpandWithDescendants(
        Guid rootId,
        IReadOnlyCollection<DocumentCategory> categories)
    {
        var active = categories.Where(category => !category.IsDeleted).ToList();
        if (active.All(category => category.Id != rootId))
        {
            throw new InvalidOperationException("Danh mục tài liệu không tồn tại hoặc đã bị xóa.");
        }

        var result = new HashSet<Guid> { rootId };
        var pending = new Queue<Guid>();
        pending.Enqueue(rootId);

        while (pending.Count > 0)
        {
            var parentId = pending.Dequeue();
            foreach (var child in active.Where(category => category.ParentId == parentId))
            {
                if (result.Add(child.Id))
                {
                    pending.Enqueue(child.Id);
                }
            }
        }

        return result;
    }

    private static int GetSubtreeDepth(
        Guid categoryId,
        IReadOnlyCollection<DocumentCategory> categories,
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

    private static List<DocumentCategoryDto> BuildChildren(
        Guid parentId,
        IReadOnlyDictionary<Guid, List<DocumentCategory>> byParent,
        IReadOnlyDictionary<Guid, string> parentNames,
        IReadOnlyDictionary<Guid, int> documentCounts,
        HashSet<Guid> ancestors,
        int depth)
    {
        if (depth >= MaxDepth || !byParent.TryGetValue(parentId, out var children))
        {
            return new List<DocumentCategoryDto>();
        }

        var result = new List<DocumentCategoryDto>();
        foreach (var category in children)
        {
            if (!ancestors.Add(category.Id))
            {
                continue;
            }

            var dto = Map(
                category,
                parentNames.GetValueOrDefault(category.ParentId ?? Guid.Empty) ?? string.Empty,
                documentCounts.GetValueOrDefault(category.Id));
            dto.SubCategories = BuildChildren(
                category.Id,
                byParent,
                parentNames,
                documentCounts,
                ancestors,
                depth + 1);
            dto.DocumentCount += dto.SubCategories.Sum(child => child.DocumentCount);
            ancestors.Remove(category.Id);
            result.Add(dto);
        }

        return result;
    }

    public static int CountDocumentsIncludingDescendants(
        Guid categoryId,
        IReadOnlyCollection<DocumentCategory> categories,
        IReadOnlyDictionary<Guid, int> directDocumentCounts)
    {
        var expandedIds = ExpandWithDescendants(categoryId, categories);
        return expandedIds.Sum(id => directDocumentCounts.GetValueOrDefault(id));
    }
}
