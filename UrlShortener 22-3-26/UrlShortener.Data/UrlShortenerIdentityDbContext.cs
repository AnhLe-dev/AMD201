using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Common.Constants;
using UrlShortener.Data.Entities.Identity;

namespace UrlShortener.Data
{
    public class UrlShortenerIdentityDbContext : IdentityDbContext<UrlShortenerUser>
    {
        public UrlShortenerIdentityDbContext(DbContextOptions<UrlShortenerIdentityDbContext> options)
            : base(options)
        {
        }

        public DbSet<UrlShortenerUser> UrlShortenerUsers { get; set; }
        public DbSet<UrlShortenerRole> UrlShortenerRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UrlShortenerUser>()
                .Property(p => p.FullName)
                .HasMaxLength(MaxLengths.USER_NAME);

            modelBuilder.Entity<UrlShortenerRole>()
                .Property(p => p.Description)
                .HasMaxLength(MaxLengths.DESCRIPTION);
        }
    }
}
