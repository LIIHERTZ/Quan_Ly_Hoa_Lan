using System;
using System.Collections.Generic;

namespace QuanLyHoaLan.Application.DTOs.Orchid;

public class SimpleCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class OrchidDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string EnglishName { get; set; } = string.Empty;
    public List<SimpleCategoryDto> Categories { get; set; } = new();
    public string ShortDescription { get; set; } = string.Empty;
    public string DetailedDescription { get; set; } = string.Empty;
    public bool HasFragrance { get; set; }
    public bool IsPopular { get; set; }
    public List<FlowerColor> Colors { get; set; } = new();
    public List<Region> Regions { get; set; } = new();
    public List<BloomSeason> BloomSeasons { get; set; } = new();
    public string Slug { get; set; } = string.Empty;
    public List<Guid> UploadedImageIds { get; set; } = new List<Guid>();
    public int DisplayOrder { get; set; }
}
