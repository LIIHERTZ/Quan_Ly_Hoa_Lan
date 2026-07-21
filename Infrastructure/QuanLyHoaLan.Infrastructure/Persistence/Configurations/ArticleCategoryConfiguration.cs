using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuanLyHoaLan.Domain.Entities;

namespace QuanLyHoaLan.Infrastructure.Persistence.Configurations;

public class ArticleCategoryConfiguration : IEntityTypeConfiguration<ArticleCategory>
{
    public void Configure(EntityTypeBuilder<ArticleCategory> builder)
    {
        builder.HasKey(category => category.Id);

        builder.Property(category => category.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(category => category.Slug)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(category => category.Type)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.HasIndex(category => new { category.Type, category.Slug })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");

        builder.Property(category => category.Description)
            .HasMaxLength(1000);

        builder.HasOne(category => category.ParentCategory)
            .WithMany(category => category.SubCategories)
            .HasForeignKey(category => category.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(category => category.Articles)
            .WithMany(article => article.Categories)
            .UsingEntity<Dictionary<string, object>>(
                "ArticleCategoryMapping",
                join => join
                    .HasOne<Article>()
                    .WithMany()
                    .HasForeignKey("ArticleId")
                    .OnDelete(DeleteBehavior.Cascade),
                join => join
                    .HasOne<ArticleCategory>()
                    .WithMany()
                    .HasForeignKey("ArticleCategoryId")
                    .OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.ToTable("ArticleCategoryMappings");
                    join.HasKey("ArticleId", "ArticleCategoryId");
                    join.HasIndex("ArticleCategoryId");
                });
    }
}
