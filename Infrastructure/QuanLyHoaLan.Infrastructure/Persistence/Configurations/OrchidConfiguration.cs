using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuanLyHoaLan.Domain.Entities;

namespace QuanLyHoaLan.Infrastructure.Persistence.Configurations;

public class OrchidConfiguration : IEntityTypeConfiguration<Orchid>
{
    public void Configure(EntityTypeBuilder<Orchid> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Name).IsRequired().HasMaxLength(256);
        builder.Property(e => e.EnglishName).HasMaxLength(256);
        builder.Property(e => e.Slug).IsRequired().HasMaxLength(256);
        builder.HasIndex(e => e.Slug).IsUnique();
        
        builder.Property(e => e.ShortDescription).HasMaxLength(1000);
        
        builder.HasMany(e => e.Categories)
            .WithMany(c => c.Orchids);
    }
}
