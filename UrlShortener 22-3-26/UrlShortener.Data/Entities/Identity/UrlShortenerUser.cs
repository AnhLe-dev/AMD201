using Microsoft.AspNetCore.Identity;

namespace UrlShortener.Data.Entities.Identity
{
    public class UrlShortenerUser : IdentityUser
    {
        public string? FullName { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
