using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuanLyHoaLan.Domain.Entities;

namespace QuanLyHoaLan.Infrastructure.Persistence.Configurations;

public class DocumentCategoryConfiguration : IEntityTypeConfiguration<DocumentCategory>
{
    public void Configure(EntityTypeBuilder<DocumentCategory> builder)
    {
        builder.HasKey(category => category.Id);

        builder.Property(category => category.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(category => category.Description)
            .HasMaxLength(1000);

        builder.Property(category => category.Slug)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(category => category.Slug)
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");

        builder.HasOne(category => category.ParentCategory)
            .WithMany(category => category.SubCategories)
            .HasForeignKey(category => category.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
