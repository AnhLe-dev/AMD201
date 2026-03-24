using UrlShortener.Common.Enums;

namespace UrlShortener.Data.Entities
{
    public class ShortenedUrl
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string OriginalUrl { get; set; } = string.Empty;
        public string ShortCode { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ExpirationDate { get; set; }
        public int ClickCount { get; set; } = 0;
        public UrlStatus Status { get; set; } = UrlStatus.Active;

        // Foreign key (optional - for future use with Identity)
        public Guid? UserId { get; set; }

        // Navigation property
        public virtual ICollection<UrlClick> UrlClicks { get; set; } = new List<UrlClick>();
    }
}
