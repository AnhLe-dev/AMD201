using UrlShortener.Common.DTOs;

namespace UrlShortener.Data.Interfaces
{
    public interface IUrlClickService
    {
        Task<bool> RecordClick(UrlClickDTO dto);
        Task<UrlClickDTO[]?> GetClicksByUrlId(Guid urlId);
        Task<int> GetTotalClicksByUrlId(Guid urlId);
    }
}
