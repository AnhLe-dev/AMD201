using System.ComponentModel.DataAnnotations;
using UrlShortener.Common.Constants;
using UrlShortener.Common.Enums;

namespace UrlShortener.Common.DTOs
{
    public class ShortenedUrlDTO
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Original URL is required")]
        [MaxLength(MaxLengths.ORIGINAL_URL)]
        [Url(ErrorMessage = "Invalid URL format")]
        public string OriginalUrl { get; set; } = string.Empty;

        [MaxLength(MaxLengths.SHORT_CODE)]
        public string ShortCode { get; set; } = string.Empty;

        [MaxLength(MaxLengths.TITLE)]
        public string? Title { get; set; }

        [MaxLength(MaxLengths.DESCRIPTION)]
        public string? Description { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int ClickCount { get; set; }
        public UrlStatus Status { get; set; }

        public Guid? UserId { get; set; }
        public string? UserName { get; set; }

        // Computed property
        public string ShortUrl => $"https://short.ly/{ShortCode}";
        public bool IsExpired => ExpirationDate.HasValue && ExpirationDate.Value < DateTime.UtcNow;
    }
}
