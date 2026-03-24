using Microsoft.EntityFrameworkCore;
using UrlShortener.Common.DTOs;
using UrlShortener.Data;
using UrlShortener.Data.Entities;
using UrlShortener.Data.Interfaces;

namespace UrlShortener.Services.Implementations
{
    public class UrlClickService : IUrlClickService
    {
        private readonly UrlShortenerDbContext _context;

        public UrlClickService(UrlShortenerDbContext context)
        {
            _context = context;
        }

        public async Task<UrlClickDTO[]?> GetClicksByUrlId(Guid urlId)
        {
            try
            {
                var clicks = await _context.UrlClicks
                    .Where(c => c.ShortenedUrlId == urlId)
                    .Select(c => new UrlClickDTO
                    {
                        Id = c.Id,
                        ShortenedUrlId = c.ShortenedUrlId,
                        ClickedAt = c.ClickedAt,
                        IpAddress = c.IpAddress,
                        UserAgent = c.UserAgent,
                        Referrer = c.Referrer,
                        Country = c.Country,
                        City = c.City
                    })
                    .ToArrayAsync();
                return clicks;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<int> GetTotalClicksByUrlId(Guid urlId)
        {
            try
            {
                return await _context.UrlClicks
                    .Where(c => c.ShortenedUrlId == urlId)
                    .CountAsync();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<bool> RecordClick(UrlClickDTO dto)
        {
            try
            {
                var entity = new UrlClick
                {
                    ShortenedUrlId = dto.ShortenedUrlId,
                    ClickedAt = DateTime.UtcNow,
                    IpAddress = dto.IpAddress,
                    UserAgent = dto.UserAgent,
                    Referrer = dto.Referrer,
                    Country = dto.Country,
                    City = dto.City
                };

                await _context.UrlClicks.AddAsync(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
