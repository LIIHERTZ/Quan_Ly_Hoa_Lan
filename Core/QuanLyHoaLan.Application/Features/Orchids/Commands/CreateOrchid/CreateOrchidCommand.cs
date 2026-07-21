using System;
using System.Collections.Generic;
using MediatR;

namespace QuanLyHoaLan.Application.Features.Orchids.Commands.CreateOrchid;

public class CreateOrchidCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string EnglishName { get; set; } = string.Empty;
    public List<Guid> CategoryIds { get; set; } = new List<Guid>();
    public string ShortDescription { get; set; } = string.Empty;
    public string DetailedDescription { get; set; } = string.Empty;
    public bool HasFragrance { get; set; }
    public bool IsPopular { get; set; }
    public List<string> Colors { get; set; } = new();
    public List<string> Regions { get; set; } = new();
    public List<string> BloomSeasons { get; set; } = new();
    public string Slug { get; set; } = string.Empty;
    public List<Guid> UploadedImageIds { get; set; } = new List<Guid>();
    public int DisplayOrder { get; set; }
}
