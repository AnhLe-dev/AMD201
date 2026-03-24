namespace UrlShortener.Common.DTOs
{
    public class UrlClickDTO
    {
        public Guid Id { get; set; }
        public Guid ShortenedUrlId { get; set; }
        public DateTime ClickedAt { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? Referrer { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
    }
}
