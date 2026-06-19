using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuanLyHoaLan.Domain.Entities;

namespace QuanLyHoaLan.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Name).IsRequired().HasMaxLength(256);
        builder.Property(e => e.Slug).IsRequired().HasMaxLength(256);
        builder.HasIndex(e => e.Slug).IsUnique();
        
        builder.Property(e => e.Description).HasMaxLength(1000);

        builder.HasOne(e => e.ParentCategory)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(e => e.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
