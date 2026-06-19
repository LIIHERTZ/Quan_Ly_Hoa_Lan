using QuanLyHoaLan.Domain.Common;

namespace QuanLyHoaLan.Domain.Entities;

public class Orchid : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string EnglishName { get; set; } = string.Empty;
    // Many-to-Many Categories
    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
    
    public string ShortDescription { get; set; } = string.Empty;
    
    // Có thể lưu định dạng HTML hoặc nội dung dài
    public string DetailedDescription { get; set; } = string.Empty;

    public bool HasFragrance { get; set; }
    public bool IsPopular { get; set; }

    public string Slug { get; set; } = string.Empty;

    // Danh mục hình ảnh: Lưu trữ ID của các ảnh trong bảng UploadedImage
    public List<Guid> UploadedImageIds { get; set; } = new List<Guid>();

    public int DisplayOrder { get; set; }
}
