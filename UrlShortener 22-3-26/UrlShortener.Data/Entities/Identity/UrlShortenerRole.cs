using Microsoft.AspNetCore.Identity;

namespace UrlShortener.Data.Entities.Identity
{
    public class UrlShortenerRole : IdentityRole
    {
        public string? Description { get; set; }
    }
}
