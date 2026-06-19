using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuanLyHoaLan.Domain.Entities;

namespace QuanLyHoaLan.Infrastructure.Persistence.Configurations;

public class DiscussionPostConfiguration : IEntityTypeConfiguration<DiscussionPost>
{
    public void Configure(EntityTypeBuilder<DiscussionPost> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.Content)
            .IsRequired();

        builder.HasOne(x => x.Author)
            .WithMany()
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Comments)
            .WithOne(x => x.Post)
            .HasForeignKey(x => x.PostId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
