using Microsoft.EntityFrameworkCore;
using UrlShortener.Common.Constants;
using UrlShortener.Data.Configurations;
using UrlShortener.Data.Entities;

namespace UrlShortener.Data
{
    public class UrlShortenerDbContext : DbContext
    {
        public UrlShortenerDbContext(DbContextOptions<UrlShortenerDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ShortenedUrl> ShortenedUrls { get; set; }
        public virtual DbSet<UrlClick> UrlClicks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ShortenedUrlConfiguration());
            modelBuilder.ApplyConfiguration(new UrlClickConfiguration());
        }
    }
}
