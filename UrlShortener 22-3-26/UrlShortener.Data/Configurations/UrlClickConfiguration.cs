using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Data.Entities;

namespace UrlShortener.Data.Configurations
{
    public class UrlClickConfiguration : IEntityTypeConfiguration<UrlClick>
    {
        public void Configure(EntityTypeBuilder<UrlClick> builder)
        {
            builder.Property(c => c.ClickedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(c => c.IpAddress)
                .HasMaxLength(45); // IPv6 max length

            builder.Property(c => c.UserAgent)
                .HasMaxLength(500);

            builder.Property(c => c.Referrer)
                .HasMaxLength(500);

            builder.Property(c => c.Country)
                .HasMaxLength(100);

            builder.Property(c => c.City)
                .HasMaxLength(100);

            builder.HasOne(c => c.ShortenedUrl)
                .WithMany(s => s.UrlClicks)
                .HasForeignKey(c => c.ShortenedUrlId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
