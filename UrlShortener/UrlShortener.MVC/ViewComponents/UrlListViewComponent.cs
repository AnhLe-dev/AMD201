using Microsoft.AspNetCore.Mvc;
using UrlShortener.Data.Interfaces;
using UrlShortener.MVC.Models;

namespace UrlShortener.MVC.ViewComponents
{
    public class UrlListViewComponent : ViewComponent
    {
        private readonly IShortenedUrlService _urlService;

        public UrlListViewComponent(IShortenedUrlService urlService)
        {
            _urlService = urlService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var urls = await _urlService.GetAll();
            var urlVMs = urls?.Select(u => new ShortenedUrlVM(u)).ToList() ?? new List<ShortenedUrlVM>();
            return View(urlVMs);
        }
    }
}
