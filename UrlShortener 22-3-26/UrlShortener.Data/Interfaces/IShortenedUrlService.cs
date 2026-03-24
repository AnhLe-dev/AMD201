using UrlShortener.Common.DTOs;

namespace UrlShortener.Data.Interfaces
{
    public interface IShortenedUrlService
    {
        Task<bool> Create(ShortenedUrlDTO dto);
        Task<ShortenedUrlDTO?> GetById(Guid id);
        Task<ShortenedUrlDTO?> GetByShortCode(string shortCode);
        Task<ShortenedUrlDTO[]?> GetAll();
        Task<ShortenedUrlDTO[]?> GetByUserId(Guid userId);
        Task<bool> Update(ShortenedUrlDTO dto);
        Task<bool> Delete(Guid id);
        Task<bool> IncrementClickCount(Guid id);
        Task<string> GenerateUniqueShortCode();
    }
}
