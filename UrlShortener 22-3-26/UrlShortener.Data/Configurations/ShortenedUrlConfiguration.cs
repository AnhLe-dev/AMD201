using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Common.Constants;
using UrlShortener.Data.Entities;

namespace UrlShortener.Data.Configurations
{
    public class ShortenedUrlConfiguration : IEntityTypeConfiguration<ShortenedUrl>
    {
        public void Configure(EntityTypeBuilder<ShortenedUrl> builder)
        {
            builder.Property(s => s.OriginalUrl)
                .IsRequired()
                .HasMaxLength(MaxLengths.ORIGINAL_URL);

            builder.Property(s => s.ShortCode)
                .IsRequired()
                .HasMaxLength(MaxLengths.SHORT_CODE);

            builder.HasIndex(s => s.ShortCode)
                .IsUnique();

            builder.Property(s => s.Title)
                .HasMaxLength(MaxLengths.TITLE);

            builder.Property(s => s.Description)
                .HasMaxLength(MaxLengths.DESCRIPTION);

            builder.Property(s => s.CreatedDate)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(s => s.Status)
                .HasConversion<int>();
        }
    }
}
