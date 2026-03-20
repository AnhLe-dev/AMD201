using System.ComponentModel.DataAnnotations;
using UrlShortener.Common.Constants;
using UrlShortener.Common.DTOs;
using UrlShortener.Common.Enums;

namespace UrlShortener.MVC.Models
{
    public class ShortenedUrlVM
    {
        public ShortenedUrlVM()
        {
        }

        public ShortenedUrlVM(ShortenedUrlDTO dto)
        {
            Id = dto.Id;
            OriginalUrl = dto.OriginalUrl;
            ShortCode = dto.ShortCode;
            Title = dto.Title;
            Description = dto.Description;
            CreatedDate = dto.CreatedDate;
            ExpirationDate = dto.ExpirationDate;
            ClickCount = dto.ClickCount;
            Status = dto.Status;
            UserId = dto.UserId;
        }

        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Please enter the original URL")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        [MaxLength(MaxLengths.ORIGINAL_URL)]
        [Display(Name = "Original URL")]
        public string OriginalUrl { get; set; } = string.Empty;

        [MaxLength(MaxLengths.SHORT_CODE)]
        [Display(Name = "Custom Alias (optional)")]
        public string? ShortCode { get; set; }

        [MaxLength(MaxLengths.TITLE)]
        [Display(Name = "Title")]
        public string? Title { get; set; }

        [MaxLength(MaxLengths.DESCRIPTION)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Expiration Date")]
        public DateTime? ExpirationDate { get; set; }

        [Display(Name = "Clicks")]
        public int ClickCount { get; set; }

        public UrlStatus Status { get; set; }

        public Guid? UserId { get; set; }

        public string ShortUrl => $"https://short.ly/{ShortCode}";
        public bool IsExpired => ExpirationDate.HasValue && ExpirationDate.Value < DateTime.UtcNow;
    }
}
