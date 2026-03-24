namespace UrlShortener.Data.Entities
{
    public class UrlClick
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime ClickedAt { get; set; } = DateTime.UtcNow;
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? Referrer { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }

        // Foreign key
        public Guid ShortenedUrlId { get; set; }
        public virtual ShortenedUrl ShortenedUrl { get; set; } = null!;
    }
}
