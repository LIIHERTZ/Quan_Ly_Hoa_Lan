using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuanLyHoaLan.Domain.Entities;

namespace QuanLyHoaLan.Infrastructure.Persistence.Configurations;

public class JwtRefreshTokenConfiguration : IEntityTypeConfiguration<JwtRefreshToken>
{
    public void Configure(EntityTypeBuilder<JwtRefreshToken> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Token).IsRequired().HasMaxLength(200);
        builder.Property(e => e.JwtId).IsRequired().HasMaxLength(200);

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
