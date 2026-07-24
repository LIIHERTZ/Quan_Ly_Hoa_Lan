using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuanLyHoaLan.Domain.Entities;

namespace QuanLyHoaLan.Infrastructure.Persistence.Configurations;

public class AppDocumentConfiguration : IEntityTypeConfiguration<AppDocument>
{
    public void Configure(EntityTypeBuilder<AppDocument> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.OriginalName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.Extension)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.Url)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.PublicId)
            .HasMaxLength(255);

        builder.HasOne(document => document.Category)
            .WithMany(category => category.Documents)
            .HasForeignKey(document => document.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(document => document.CategoryId);
    }
}
