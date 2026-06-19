using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuanLyHoaLan.Domain.Entities;

namespace QuanLyHoaLan.Infrastructure.Persistence.Configurations;

public class UploadedImageConfiguration : IEntityTypeConfiguration<UploadedImage>
{
    public void Configure(EntityTypeBuilder<UploadedImage> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Url).IsRequired();
        builder.Property(e => e.PublicId).IsRequired().HasMaxLength(256);
        builder.Property(e => e.FileName).HasMaxLength(256);
    }
}
