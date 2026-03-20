using Microsoft.EntityFrameworkCore;
using UrlShortener.Common.DTOs;
using UrlShortener.Common.Enums;
using UrlShortener.Data;
using UrlShortener.Data.Entities;
using UrlShortener.Data.Interfaces;

namespace UrlShortener.Services.Implementations
{
    public class ShortenedUrlService : IShortenedUrlService
    {
        private readonly UrlShortenerDbContext _context;
        private const string CHARACTERS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public ShortenedUrlService(UrlShortenerDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Create(ShortenedUrlDTO dto)
        {
            try
            {
                var entity = new ShortenedUrl
                {
                    OriginalUrl = dto.OriginalUrl.Trim(),
                    ShortCode = string.IsNullOrEmpty(dto.ShortCode) 
                        ? await GenerateUniqueShortCode() 
                        : dto.ShortCode.Trim(),
                    Title = dto.Title?.Trim(),
                    Description = dto.Description?.Trim(),
                    CreatedDate = DateTime.UtcNow,
                    ExpirationDate = dto.ExpirationDate,
                    Status = UrlStatus.Active,
                    UserId = dto.UserId
                };

                await _context.ShortenedUrls.AddAsync(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> Delete(Guid id)
        {
            try
            {
                var entity = await _context.ShortenedUrls.FindAsync(id);
                if (entity != null)
                {
                    _context.ShortenedUrls.Remove(entity);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<string> GenerateUniqueShortCode()
        {
            const int codeLength = 6;
            var random = new Random();
            string shortCode;

            do
            {
                shortCode = new string(Enumerable.Range(0, codeLength)
                    .Select(_ => CHARACTERS[random.Next(CHARACTERS.Length)])
                    .ToArray());
            }
            while (await _context.ShortenedUrls.AnyAsync(u => u.ShortCode == shortCode));

            return shortCode;
        }

        public async Task<ShortenedUrlDTO[]?> GetAll()
        {
            try
            {
                var urls = await _context.ShortenedUrls
                    .Select(s => new ShortenedUrlDTO
                    {
                        Id = s.Id,
                        OriginalUrl = s.OriginalUrl,
                        ShortCode = s.ShortCode,
                        Title = s.Title,
                        Description = s.Description,
                        CreatedDate = s.CreatedDate,
                        ExpirationDate = s.ExpirationDate,
                        ClickCount = s.ClickCount,
                        Status = s.Status,
                        UserId = s.UserId
                    })
                    .ToArrayAsync();
                return urls;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ShortenedUrlDTO?> GetById(Guid id)
        {
            try
            {
                var url = await _context.ShortenedUrls
                    .Where(s => s.Id == id)
                    .Select(s => new ShortenedUrlDTO
                    {
                        Id = s.Id,
                        OriginalUrl = s.OriginalUrl,
                        ShortCode = s.ShortCode,
                        Title = s.Title,
                        Description = s.Description,
                        CreatedDate = s.CreatedDate,
                        ExpirationDate = s.ExpirationDate,
                        ClickCount = s.ClickCount,
                        Status = s.Status,
                        UserId = s.UserId
                    })
                    .FirstOrDefaultAsync();
                return url;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ShortenedUrlDTO?> GetByShortCode(string shortCode)
        {
            try
            {
                var url = await _context.ShortenedUrls
                    .Where(s => s.ShortCode == shortCode)
                    .Select(s => new ShortenedUrlDTO
                    {
                        Id = s.Id,
                        OriginalUrl = s.OriginalUrl,
                        ShortCode = s.ShortCode,
                        Title = s.Title,
                        Description = s.Description,
                        CreatedDate = s.CreatedDate,
                        ExpirationDate = s.ExpirationDate,
                        ClickCount = s.ClickCount,
                        Status = s.Status,
                        UserId = s.UserId
                    })
                    .FirstOrDefaultAsync();
                return url;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ShortenedUrlDTO[]?> GetByUserId(Guid userId)
        {
            try
            {
                var urls = await _context.ShortenedUrls
                    .Where(s => s.UserId == userId)
                    .Select(s => new ShortenedUrlDTO
                    {
                        Id = s.Id,
                        OriginalUrl = s.OriginalUrl,
                        ShortCode = s.ShortCode,
                        Title = s.Title,
                        Description = s.Description,
                        CreatedDate = s.CreatedDate,
                        ExpirationDate = s.ExpirationDate,
                        ClickCount = s.ClickCount,
                        Status = s.Status,
                        UserId = s.UserId
                    })
                    .ToArrayAsync();
                return urls;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> IncrementClickCount(Guid id)
        {
            try
            {
                var entity = await _context.ShortenedUrls.FindAsync(id);
                if (entity != null)
                {
                    entity.ClickCount++;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> Update(ShortenedUrlDTO dto)
        {
            try
            {
                var entity = await _context.ShortenedUrls.FindAsync(dto.Id);
                if (entity != null)
                {
                    entity.OriginalUrl = dto.OriginalUrl.Trim();
                    entity.Title = dto.Title?.Trim();
                    entity.Description = dto.Description?.Trim();
                    entity.ExpirationDate = dto.ExpirationDate;
                    entity.Status = dto.Status;

                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
