using System;
using System.Collections.Generic;

namespace QuanLyHoaLan.Application.DTOs.Category;

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public string ParentName { get; set; } = string.Empty;

    public List<CategoryDto> SubCategories { get; set; } = new List<CategoryDto>();
}
